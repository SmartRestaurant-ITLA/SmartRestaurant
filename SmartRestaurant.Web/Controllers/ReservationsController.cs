using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Web.ViewModels;

namespace SmartRestaurant.Web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ReservationService _reservationService;
        private readonly IReservationStore _reservationStore;

        public ReservationsController(
            ReservationService reservationService,
            IReservationStore reservationStore)
        {
            _reservationService = reservationService;
            _reservationStore = reservationStore;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var tables = _reservationStore.GetTables()
                .ToDictionary(table => table.Id);

            var reservations = _reservationStore.GetReservations()
                .OrderByDescending(reservation => reservation.StartAt)
                .Select(reservation => ToListItemViewModel(reservation, tables))
                .ToList();

            return View(reservations);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateReservationViewModel
            {
                StartAt = DateTime.Now.AddHours(1)
            };

            ReloadAvailableTables(model, null, false);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateReservationViewModel model, string command)
        {
            if (string.Equals(command, "suggest", StringComparison.OrdinalIgnoreCase))
            {
                ReloadAvailableTables(model, null, true);
                return View(model);
            }

            if (!string.Equals(command, "confirm", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ReloadAvailableTables(model, null, true);
                return View(model);
            }

            if (!model.TableId.HasValue)
            {
                ModelState.AddModelError(nameof(model.TableId), "Selecciona una mesa disponible.");
                ReloadAvailableTables(model, null, true);
                return View(model);
            }

            try
            {
                var reservation = _reservationService.SaveReservation(
                    model.CustomerName,
                    model.StartAt,
                    model.DurationMinutes,
                    model.PartySize,
                    model.TableId.Value);

                reservation = _reservationService.ConfirmReservation(reservation.Id);
                TempData["Success"] = "La reserva se confirmó correctamente.";
                return RedirectToAction(nameof(Details), new { id = reservation.Id });
            }
            catch (Exception exception) when (IsReservationValidationException(exception))
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                ReloadAvailableTables(model, null, true);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var reservation = _reservationStore.GetReservation(id);
            if (reservation is null)
            {
                return NotFound();
            }

            var tables = _reservationStore.GetTables()
                .ToDictionary(table => table.Id);

            return View(ToListItemViewModel(reservation, tables));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var reservation = _reservationStore.GetReservation(id);
            if (reservation is null)
            {
                return NotFound();
            }

            var model = new EditReservationViewModel
            {
                Id = reservation.Id,
                CustomerName = reservation.CustomerName,
                StartAt = reservation.StartAt,
                PartySize = reservation.PartySize,
                DurationMinutes = reservation.DurationMinutes,
                TableId = reservation.TableId
            };

            ReloadAvailableTables(model, reservation.Id, false);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditReservationViewModel model, string command)
        {
            var reservation = _reservationStore.GetReservation(model.Id);
            if (reservation is null)
            {
                return NotFound();
            }

            if (string.Equals(command, "suggest", StringComparison.OrdinalIgnoreCase))
            {
                ReloadAvailableTables(model, model.Id, true);
                return View(model);
            }

            if (!string.Equals(command, "save", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ReloadAvailableTables(model, model.Id, true);
                return View(model);
            }

            if (!model.TableId.HasValue)
            {
                ModelState.AddModelError(nameof(model.TableId), "Selecciona una mesa disponible.");
                ReloadAvailableTables(model, model.Id, true);
                return View(model);
            }

            try
            {
                var updatedReservation = _reservationService.SaveReservation(
                    model.CustomerName,
                    model.StartAt,
                    model.DurationMinutes,
                    model.PartySize,
                    model.TableId.Value,
                    model.Id);

                TempData["Success"] = "Los cambios de la reserva se guardaron correctamente.";
                return RedirectToAction(nameof(Details), new { id = updatedReservation.Id });
            }
            catch (Exception exception) when (IsReservationValidationException(exception))
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                ReloadAvailableTables(model, model.Id, true);
                return View(model);
            }
        }

        private void ReloadAvailableTables(
            CreateReservationViewModel model,
            int? excludedReservationId,
            bool suggestTable)
        {
            model.AvailableTables = [];

            if (model.DurationMinutes <= 0 || model.PartySize <= 0)
            {
                model.SuggestedTableId = null;
                model.SuggestedTableDescription = null;
                return;
            }

            try
            {
                var availableTables = _reservationService.GetAvailableTables(
                    model.StartAt,
                    model.DurationMinutes,
                    model.PartySize,
                    excludedReservationId);

                model.AvailableTables = availableTables
                    .Select(table => new SelectListItem(
                        $"Mesa {table.Number} · capacidad {table.Capacity}",
                        table.Id.ToString(),
                        model.TableId == table.Id))
                    .ToList();

                if (!suggestTable)
                {
                    return;
                }

                var suggestion = availableTables.FirstOrDefault();
                model.SuggestedTableId = suggestion?.Id;
                model.SuggestedTableDescription = suggestion is null
                    ? null
                    : $"Mesa {suggestion.Number} · capacidad {suggestion.Capacity}";
                model.TableId = suggestion?.Id;
            }
            catch (ArgumentOutOfRangeException)
            {
                model.AvailableTables = [];
                model.SuggestedTableId = null;
                model.SuggestedTableDescription = null;
            }
        }

        private static ReservationListItemViewModel ToListItemViewModel(
            Reservation reservation,
            IReadOnlyDictionary<int, Table> tables)
        {
            var tableNumber = tables.TryGetValue(reservation.TableId, out var table)
                ? table.Number
                : reservation.TableId;

            return new ReservationListItemViewModel
            {
                Id = reservation.Id,
                CustomerName = reservation.CustomerName,
                StartAt = reservation.StartAt,
                PartySize = reservation.PartySize,
                DurationMinutes = reservation.DurationMinutes,
                TableNumber = tableNumber,
                Status = GetStatusLabel(reservation.Status)
            };
        }

        private static string GetStatusLabel(ReservationStatus status)
        {
            return status switch
            {
                ReservationStatus.Pending => "Pendiente",
                ReservationStatus.Confirmed => "Confirmada",
                ReservationStatus.Completed => "Completada",
                ReservationStatus.Cancelled => "Cancelada",
                _ => "Desconocido"
            };
        }

        private static bool IsReservationValidationException(Exception exception)
        {
            return exception is InvalidOperationException
                or ArgumentException
                or KeyNotFoundException;
        }
    }
}

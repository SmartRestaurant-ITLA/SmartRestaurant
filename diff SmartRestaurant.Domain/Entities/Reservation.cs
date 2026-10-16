@@
     public class Reservation
     {
+        private const int CustomerNameMaxLength = 120;
+
         public int Id { get; private set; }
@@
             ReservationStatus status = ReservationStatus.Pending)
         {
-            ArgumentNullException.ThrowIfNull(customerName);
-
-            if (durationMinutes <= 0)
-            {
-                throw new ArgumentOutOfRangeException(
-                    nameof(durationMinutes),
-                    "La duración debe ser positiva.");
-            }
-
-            if (partySize <= 0)
-            {
-                throw new ArgumentOutOfRangeException(
-                    nameof(partySize),
-                    "El tamaño del grupo debe ser positivo.");
-            }
+            ValidateDetails(customerName, durationMinutes, partySize);

             if (!Enum.IsDefined(status))
             {
@@
             Status = status;
         }

+        public void AssignId(int id)
+        {
+            if (id <= 0)
+            {
+                throw new ArgumentOutOfRangeException(
+                    nameof(id),
+                    "El identificador debe ser positivo.");
+            }
+
+            if (Id != 0)
+            {
+                throw new InvalidOperationException(
+                    "El identificador de la reserva ya fue asignado.");
+            }
+
+            // La identidad la proporciona quien crea la reserva; así, la entidad no depende de cómo se generan los IDs.
+            Id = id;
+        }
+
+        public void UpdateDetails(
+            string customerName,
+            DateTime startAt,
+            int durationMinutes,
+            int partySize,
+            int tableId)
+        {
+            // Validar todos los datos antes de la primera asignación evita dejar la entidad parcialmente modificada.
+            ValidateDetails(customerName, durationMinutes, partySize);
+
+            CustomerName = customerName;
+            StartAt = startAt;
+            DurationMinutes = durationMinutes;
+            PartySize = partySize;
+            TableId = tableId;
+        }
+
         public void ChangeStatus(ReservationStatus status)
         {
             if (!Enum.IsDefined(status))
@@

             Status = status;
         }
+
+        private static void ValidateDetails(
+            string customerName,
+            int durationMinutes,
+            int partySize)
+        {
+            ArgumentNullException.ThrowIfNull(customerName);
+
+            // Estas reglas corresponden al Required y StringLength(120) del formulario.
+            if (string.IsNullOrWhiteSpace(customerName))
+            {
+                throw new ArgumentException(
+                    "El nombre del cliente es obligatorio.",
+                    nameof(customerName));
+            }
+
+            if (customerName.Length > CustomerNameMaxLength)
+            {
+                throw new ArgumentException(
+                    $"El nombre del cliente no puede superar los {CustomerNameMaxLength} caracteres.",
+                    nameof(customerName));
+            }
+
+            if (durationMinutes <= 0)
+            {
+                throw new ArgumentOutOfRangeException(
+                    nameof(durationMinutes),
+                    "La duración debe ser positiva.");
+            }
+
+            if (partySize <= 0)
+            {
+                throw new ArgumentOutOfRangeException(
+                    nameof(partySize),
+                    "El tamaño del grupo debe ser positivo.");
+            }
+        }
     }
 }
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const detalle = extraerMensaje(error);
      snackBar.open(detalle, 'Cerrar', {
        duration: 5000,
        panelClass: 'snack-error'
      });
      return throwError(() => error);
    })
  );
};

function extraerMensaje(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'No fue posible conectar con la API. Verifique que el backend esté en ejecución.';
  }

  const cuerpo = error.error as { detail?: string; title?: string; errors?: Record<string, string[]> } | string | null;

  if (typeof cuerpo === 'string' && cuerpo.trim()) {
    return cuerpo;
  }

  if (cuerpo && typeof cuerpo === 'object') {
    if (cuerpo.errors) {
      const primero = Object.values(cuerpo.errors).flat()[0];
      if (primero) {
        return primero;
      }
    }

    if (cuerpo.detail) {
      return cuerpo.detail;
    }
  }

  return 'Ocurrió un error al procesar la solicitud.';
}

import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const fechaNoFutura: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const valor = control.value as string | null;
  if (!valor) {
    return null;
  }

  const fecha = new Date(`${valor}T00:00:00`);
  const hoy = new Date();
  hoy.setHours(0, 0, 0, 0);

  return fecha > hoy ? { fechaFutura: true } : null;
};

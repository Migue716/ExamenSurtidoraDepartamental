import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ClienteEscritura } from '../../models/cliente.model';
import { ClienteService } from '../../services/cliente.service';
import { fechaNoFutura } from '../../../shared/validadores/fecha-no-futura.validator';

@Component({
  selector: 'app-cliente-formulario',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './cliente-formulario.component.html',
  styleUrl: './cliente-formulario.component.scss'
})
export class ClienteFormularioComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly ruta = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly servicio = inject(ClienteService);
  private readonly snackBar = inject(MatSnackBar);

  readonly id = signal<number | null>(null);
  readonly guardarIntento = signal(false);
  readonly guardando = signal(false);

  readonly form = this.fb.nonNullable.group({
    nombre: ['', [Validators.required, Validators.maxLength(100)]],
    apellidoPaterno: ['', [Validators.required, Validators.maxLength(100)]],
    apellidoMaterno: ['', Validators.maxLength(100)],
    correoElectronico: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
    telefono: ['', Validators.maxLength(20)],
    fechaNacimiento: ['', fechaNoFutura],
    direccion: ['', Validators.maxLength(250)],
    ciudad: ['', Validators.maxLength(100)],
    codigoPostal: ['', Validators.maxLength(10)]
  });

  get esEdicion(): boolean {
    return this.id() !== null;
  }

  ngOnInit(): void {
    const parametro = this.ruta.snapshot.paramMap.get('id');
    if (!parametro) {
      return;
    }

    const id = Number(parametro);
    this.id.set(id);
    this.servicio.obtener(id).subscribe((cliente) => {
      this.form.patchValue({
        nombre: cliente.nombre,
        apellidoPaterno: cliente.apellidoPaterno,
        apellidoMaterno: cliente.apellidoMaterno ?? '',
        correoElectronico: cliente.correoElectronico,
        telefono: cliente.telefono ?? '',
        fechaNacimiento: cliente.fechaNacimiento ?? '',
        direccion: cliente.direccion ?? '',
        ciudad: cliente.ciudad ?? '',
        codigoPostal: cliente.codigoPostal ?? ''
      });
    });
  }

  mensajeError(controlNombre: string): string {
    const control = this.form.get(controlNombre);
    if (!control || (!control.touched && !this.guardarIntento())) {
      return '';
    }

    if (control.hasError('required')) {
      return 'Este campo es obligatorio.';
    }
    if (control.hasError('email')) {
      return 'Ingrese un correo electrónico válido.';
    }
    if (control.hasError('maxlength')) {
      const requerido = control.getError('maxlength')?.requiredLength;
      return `No puede superar ${requerido} caracteres.`;
    }
    if (control.hasError('fechaFutura')) {
      return 'La fecha de nacimiento no puede ser futura.';
    }
    return '';
  }

  guardar(): void {
    this.guardarIntento.set(true);
    if (this.form.invalid || this.guardando()) {
      this.form.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    const dto = this.construirDto();
    const peticion = this.esEdicion
      ? this.servicio.actualizar(this.id()!, dto)
      : this.servicio.crear(dto);

    peticion.subscribe({
      next: () => {
        this.snackBar.open(
          this.esEdicion ? 'El cliente fue actualizado.' : 'El cliente fue registrado.',
          'Cerrar',
          { panelClass: 'snack-ok' }
        );
        void this.router.navigate(['/clientes']);
      },
      error: () => this.guardando.set(false)
    });
  }

  private construirDto(): ClienteEscritura {
    const valor = this.form.getRawValue();
    return {
      nombre: valor.nombre.trim(),
      apellidoPaterno: valor.apellidoPaterno.trim(),
      apellidoMaterno: valor.apellidoMaterno.trim() || null,
      correoElectronico: valor.correoElectronico.trim(),
      telefono: valor.telefono.trim() || null,
      fechaNacimiento: valor.fechaNacimiento || null,
      direccion: valor.direccion.trim() || null,
      ciudad: valor.ciudad.trim() || null,
      codigoPostal: valor.codigoPostal.trim() || null
    };
  }
}

import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

export interface ConfirmacionDialogoData {
  titulo: string;
  mensaje: string;
  textoConfirmar?: string;
}

@Component({
  selector: 'app-confirmacion-dialogo',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data.titulo }}</h2>
    <mat-dialog-content>
      <p>{{ data.mensaje }}</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="dialogo.close(false)">Cancelar</button>
      <button mat-flat-button color="warn" type="button" (click)="dialogo.close(true)">
        {{ data.textoConfirmar || 'Confirmar' }}
      </button>
    </mat-dialog-actions>
  `
})
export class ConfirmacionDialogoComponent {
  readonly dialogo = inject(MatDialogRef<ConfirmacionDialogoComponent, boolean>);
  readonly data = inject<ConfirmacionDialogoData>(MAT_DIALOG_DATA);
}

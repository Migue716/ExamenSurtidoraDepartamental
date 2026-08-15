import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Cliente,
  ClienteEscritura,
  ClienteFiltro,
  ResultadoPaginado
} from '../models/cliente.model';

@Injectable({ providedIn: 'root' })
export class ClienteService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/clientes`;

  consultar(filtro: ClienteFiltro): Observable<ResultadoPaginado<Cliente>> {
    let params = new HttpParams()
      .set('pagina', filtro.pagina)
      .set('tamanioPagina', filtro.tamanioPagina);

    if (filtro.buscar?.trim()) {
      params = params.set('buscar', filtro.buscar.trim());
    }

    if (filtro.activo !== undefined && filtro.activo !== null) {
      params = params.set('activo', String(filtro.activo));
    }

    if (filtro.ordenarPor) {
      params = params.set('ordenarPor', filtro.ordenarPor);
    }

    if (filtro.descendente) {
      params = params.set('descendente', 'true');
    }

    return this.http.get<ResultadoPaginado<Cliente>>(this.url, { params });
  }

  obtener(id: number): Observable<Cliente> {
    return this.http.get<Cliente>(`${this.url}/${id}`);
  }

  crear(dto: ClienteEscritura): Observable<Cliente> {
    return this.http.post<Cliente>(this.url, dto);
  }

  actualizar(id: number, dto: ClienteEscritura): Observable<Cliente> {
    return this.http.put<Cliente>(`${this.url}/${id}`, dto);
  }

  darDeBaja(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}

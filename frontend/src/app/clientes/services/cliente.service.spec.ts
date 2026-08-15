import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { ClienteService } from './cliente.service';
import { ClienteEscritura, ResultadoPaginado, Cliente } from '../models/cliente.model';

describe('ClienteService', () => {
  let servicio: ClienteService;
  let http: HttpTestingController;
  const url = `${environment.apiUrl}/clientes`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), ClienteService]
    });

    servicio = TestBed.inject(ClienteService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('consulta clientes con paginación y búsqueda', () => {
    const respuesta: ResultadoPaginado<Cliente> = {
      items: [],
      pagina: 1,
      tamanioPagina: 10,
      totalRegistros: 0,
      totalPaginas: 0
    };

    servicio.consultar({ pagina: 1, tamanioPagina: 10, buscar: 'David', activo: true }).subscribe((resultado) => {
      expect(resultado.totalRegistros).toBe(0);
    });

    const peticion = http.expectOne(
      (req) =>
        req.method === 'GET' &&
        req.url === url &&
        req.params.get('pagina') === '1' &&
        req.params.get('buscar') === 'David' &&
        req.params.get('activo') === 'true'
    );
    peticion.flush(respuesta);
  });

  it('envía el alta a POST /clientes', () => {
    const dto: ClienteEscritura = {
      nombre: 'Laura',
      apellidoPaterno: 'Martínez',
      correoElectronico: 'laura.martinez@correo.com'
    };

    servicio.crear(dto).subscribe((cliente) => {
      expect(cliente.clienteId).toBe(1);
    });

    const peticion = http.expectOne(url);
    expect(peticion.request.method).toBe('POST');
    expect(peticion.request.body).toEqual(dto);
    peticion.flush({
      clienteId: 1,
      ...dto,
      activo: true,
      fechaRegistro: '2026-08-15T00:00:00Z'
    });
  });

  it('solicita baja lógica con DELETE', () => {
    servicio.darDeBaja(7).subscribe();
    const peticion = http.expectOne(`${url}/7`);
    expect(peticion.request.method).toBe('DELETE');
    peticion.flush(null);
  });
});

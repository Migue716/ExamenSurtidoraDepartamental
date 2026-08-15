export interface Cliente {
  clienteId: number;
  nombre: string;
  apellidoPaterno: string;
  apellidoMaterno?: string | null;
  correoElectronico: string;
  telefono?: string | null;
  fechaNacimiento?: string | null;
  direccion?: string | null;
  ciudad?: string | null;
  codigoPostal?: string | null;
  activo: boolean;
  fechaRegistro: string;
  fechaModificacion?: string | null;
}

export interface ClienteEscritura {
  nombre: string;
  apellidoPaterno: string;
  apellidoMaterno?: string | null;
  correoElectronico: string;
  telefono?: string | null;
  fechaNacimiento?: string | null;
  direccion?: string | null;
  ciudad?: string | null;
  codigoPostal?: string | null;
}

export interface ResultadoPaginado<T> {
  items: T[];
  pagina: number;
  tamanioPagina: number;
  totalRegistros: number;
  totalPaginas: number;
}

export interface ClienteFiltro {
  pagina: number;
  tamanioPagina: number;
  buscar?: string;
  activo?: boolean | null;
  ordenarPor?: string;
  descendente?: boolean;
}

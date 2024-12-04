// ************************************************************************
// Practica 07
// Alex Calderon
// Fecha de realización: 02/08/2024
// Fecha de entrega: 09/08/2024

// Resultados:
//  Ventajas y Desventajas de .NET Framework y .NET Core

//  .NET Framework
//  Ventajas
//  -Incluye diferentes herramientas y lenguajes de programación lo que facilita su desarrollo.
//  -Este Framework brinda una mayor estabilidad y mejora en sus funciones para aplicaciones que ya existen.
//  Desventajas
//  Posee la limitación de que sólo se puede usar en sistemas operativos Windows, lo cual impide su uso en diferentes sistemas operativos.
//  -NET Framework no es muy rentable debido a su gran peso y dificultad para mantenerlo.

//  .NET Core:
//  Ventajas
//  -Ofrece una mayor flexibilidad porque permite el desarrollo de aplicaciones en Windows, Linux y MacOS.
//  -Brinda a los desarrolladores la oportunidad de acceder al código abierto y fomenta la colaboración al tener una comunidad sólida.
//  -Permite la descarga de los recursos o tecnologías necesarias, en vez de tener la necesidad de descargar el paquete completo.

// Acerca del Codigo
// Codigo que permite realizar una tabla con potencias y una tabla con ángulos 
// y los valores correspondientes a las funciones seno y coseno
// Conclusiones:
// * La implementación de diferentes métodos para realizar los cálculos respectivos
// mejora la organización y claridad del código.
// * Es relevante el uso de constantes cuando se van a emplear valores que no cambian,
// debido a que, ayuda a evitar errores de tipeo y mejora la comprensión del código. 
// Recomendaciones:
// * Se recomienda mejorar el formato o estructura de visualziación de los resultados
// obtenido por medio de una alineación adecuada y separadores que faciliten la comprensión
// de la información presentada.
// ************************************************************************



using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Protocolo
{
    public class Pedido
    {
        public string Comando { get; set; }
        public string[] Parametros { get; set; }

        public static Pedido Procesar(string mensaje)
        {
            // Procesa el mensaje recibido y lo convierte en un objeto Pedido
            var partes = mensaje.Split(' ');
            return new Pedido
            {
                Comando = partes[0].ToUpper(),
                Parametros = partes.Skip(1).ToArray()
            };
        }

        public override string ToString()
        {
            return $"{Comando} {string.Join(" ", Parametros)}";
        }
    }

    public class Respuesta
    {
        public string Estado { get; set; }
        public string Mensaje { get; set; }

        public override string ToString()
        {
            return $"{Estado} {Mensaje}";
        }
    }

    public static class Protocolo
    {
        private static Dictionary<string, int> listadoClientes = new Dictionary<string, int>();

        public static Respuesta ResolverPedido(Pedido pedido, string direccionCliente)
        {
            Respuesta respuesta = new Respuesta
            { Estado = "NOK", Mensaje = "Comando no reconocido" };

            switch (pedido.Comando)
            {
                case "INGRESO":
                    // Verifica las credenciales del usuario
                    if (pedido.Parametros.Length == 2 &&
                        pedido.Parametros[0] == "root" &&
                        pedido.Parametros[1] == "admin20")
                    {
                        respuesta = new Random().Next(2) == 0
                            ? new Respuesta
                            {
                                Estado = "OK",
                                Mensaje = "ACCESO_CONCEDIDO"
                            }
                            : new Respuesta
                            {
                                Estado = "NOK",
                                Mensaje = "ACCESO_NEGADO"
                            };
                    }
                    else
                    {
                        respuesta.Mensaje = "ACCESO_NEGADO";
                    }
                    break;

                case "CALCULO":
                    // Realiza el cálculo basado en la placa del vehículo
                    if (pedido.Parametros.Length == 3)
                    {
                        string modelo = pedido.Parametros[0];
                        string marca = pedido.Parametros[1];
                        string placa = pedido.Parametros[2];
                        if (ValidarPlaca(placa))
                        {
                            byte indicadorDia = ObtenerIndicadorDia(placa);
                            respuesta = new Respuesta
                            {
                                Estado = "OK",
                                Mensaje = $"{placa} {indicadorDia}"
                            };
                            ContadorCliente(direccionCliente);
                        }
                        else
                        {
                            respuesta.Mensaje = "Placa no válida";
                        }
                    }
                    break;

                case "CONTADOR":
                    // Devuelve el número de solicitudes del cliente
                    if (listadoClientes.ContainsKey(direccionCliente))
                    {
                        respuesta = new Respuesta
                        {
                            Estado = "OK",
                            Mensaje = listadoClientes[direccionCliente].ToString()
                        };
                    }
                    else
                    {
                        respuesta.Mensaje = "No hay solicitudes previas";
                    }
                    break;
            }

            return respuesta;
        }

        public static Respuesta ProcesarRespuesta(string mensaje)
        {
            // Procesa el mensaje recibido y lo convierte en un objeto Respuesta
            var partes = mensaje.Split(' ');
            return new Respuesta
            {
                Estado = partes[0],
                Mensaje = string.Join(" ", partes.Skip(1).ToArray())
            };
        }

        private static bool ValidarPlaca(string placa)
        {
            // Valida el formato de la placa
            return Regex.IsMatch(placa, @"^[A-Z]{3}[0-9]{4}$");
        }

        private static byte ObtenerIndicadorDia(string placa)
        {
            // Obtiene el indicador del día basado en el último dígito de la placa
            int ultimoDigito = int.Parse(placa.Substring(6, 1));
            switch (ultimoDigito)
            {
                case 1:
                case 2:
                    return 0b00100000; // Lunes
                case 3:
                case 4:
                    return 0b00010000; // Martes
                case 5:
                case 6:
                    return 0b00001000; // Miércoles
                case 7:
                case 8:
                    return 0b00000100; // Jueves
                case 9:
                case 0:
                    return 0b00000010; // Viernes
                default:
                    return 0;
            }
        }

        private static void ContadorCliente(string direccionCliente)
        {
            // Incrementa el contador de solicitudes del cliente
            if (listadoClientes.ContainsKey(direccionCliente))
            {
                listadoClientes[direccionCliente]++;
            }
            else
            {
                listadoClientes[direccionCliente] = 1;
            }
        }
    }
}

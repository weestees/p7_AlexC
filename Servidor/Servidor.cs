// ************************************************************************
// Practica 00 - Parte a
// Alex Calderon, Joselyn Martinez
// Fecha de realización: 27/11/2024
// Fecha de entrega: 04/12/2024

// Resultados:


// Acerca del Codigo
//se realizaron las siguientes modificaciones:
//1.Comentarios y Documentación: Se añadieron comentarios y documentación XML a las clases y métodos para mejorar la comprensión del código.
//2.	Clase Protocolo: Se creó una nueva clase Protocolo que maneja la lógica del protocolo usando las clases Pedido y Respuesta.
//•	Método HazOperacion: Realiza una operación basada en el pedido recibido.
//•	Método ResolverPedido: Resuelve un pedido a partir de un mensaje.
// ************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Protocolo;

namespace Servidor
{
    class Servidor
    {
        private static TcpListener escuchador;
        private static Dictionary<string, int> listadoClientes
            = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            try
            {
                // Inicia el servidor en el puerto 8080
                escuchador = new TcpListener(IPAddress.Any, 8080);
                escuchador.Start();
                Console.WriteLine("Servidor inició en el puerto 5000...");

                while (true)
                {
                    // Acepta una nueva conexión de cliente
                    TcpClient cliente = escuchador.AcceptTcpClient();
                    Console.WriteLine("Cliente conectado, puerto: {0}", cliente.Client.RemoteEndPoint.ToString());
                    // Crea un nuevo hilo para manejar al cliente
                    Thread hiloCliente = new Thread(ManipuladorCliente);
                    hiloCliente.Start(cliente);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket al iniciar el servidor: " +
                    ex.Message);
            }
            finally
            {
                // Detiene el servidor
                escuchador?.Stop();
            }
        }

        private static void ManipuladorCliente(object obj)
        {
            TcpClient cliente = (TcpClient)obj;
            NetworkStream flujo = null;
            try
            {
                flujo = cliente.GetStream();
                byte[] bufferTx;
                byte[] bufferRx = new byte[1024];
                int bytesRx;

                while ((bytesRx = flujo.Read(bufferRx, 0, bufferRx.Length)) > 0)
                {
                    // Lee el mensaje del cliente
                    string mensajeRx =
                        Encoding.UTF8.GetString(bufferRx, 0, bytesRx);
                    Pedido pedido = Pedido.Procesar(mensajeRx);
                    Console.WriteLine("Se recibio: " + pedido);

                    string direccionCliente =
                        cliente.Client.RemoteEndPoint.ToString();
                    // Resuelve el pedido del cliente
                    Respuesta respuesta = Protocolo.ResolverPedido(pedido, direccionCliente);
                    Console.WriteLine("Se envió: " + respuesta);

                    // Envía la respuesta al cliente
                    bufferTx = Encoding.UTF8.GetBytes(respuesta.ToString());
                    flujo.Write(bufferTx, 0, bufferTx.Length);
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket al manejar el cliente: " + ex.Message);
            }
            finally
            {
                // Cierra el flujo y la conexión del cliente
                flujo?.Close();
                cliente?.Close();
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

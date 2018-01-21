using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_WSA && !UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Networking;
using System.Diagnostics;
#endif

public class MeshReceiver : MonoBehaviour {

    static string PortNumber = "11000";

    private bool clientConnected = false;

    private string message = "Start";

#if UNITY_WSA && !UNITY_EDITOR
    private Windows.Networking.Sockets.StreamSocketListener streamSocketListener;
#endif

    // Use this for initialization
    void Start () {
        
#if UNITY_WSA && !UNITY_EDITOR
        StartServer();
#endif
    }

    // Update is called once per frame
    void Update () {
#if UNITY_WSA && !UNITY_EDITOR
        
#endif
        GetComponent<TextMesh>().text = message;
    }

#if UNITY_WSA && !UNITY_EDITOR
    private async void StartServer()
    {
        try
        {
            streamSocketListener = new Windows.Networking.Sockets.StreamSocketListener();

            // The ConnectionReceived event is raised when connections are received.
            streamSocketListener.ConnectionReceived += this.StreamSocketListener_ConnectionReceived;

            // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
            await streamSocketListener.BindServiceNameAsync(PortNumber);

            message = $"Listener started on port {PortNumber}";
        }
        catch (Exception ex)
        {
            message = "Couldn't open socket listener.";
        }
    }
#endif

    /// <summary>
    /// Reads an int from the next 4 bytes of the supplied stream.
    /// </summary>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <returns>An integer representing the bytes.</returns>
    private int ReadInt(Stream stream)
    {
        // The bytes arrive in the wrong order, so swap them.
        byte[] bytes = new byte[4];
        stream.Read(bytes, 0, 4);
        byte t = bytes[0];
        bytes[0] = bytes[3];
        bytes[3] = t;

        t = bytes[1];
        bytes[1] = bytes[2];
        bytes[2] = t;

        // Then bitconverter can read the int32.
        return BitConverter.ToInt32(bytes, 0);

    }

#if UNITY_WSA && !UNITY_EDITOR
    private async void StopServer()
    {
        if (streamSocketListener != null)
        {
            streamSocketListener.Dispose();
            message = "Listener stopped.";
        }
    }
#endif

#if UNITY_WSA && !UNITY_EDITOR
    private async void StreamSocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
    {
        //if (!clientConnected)
        //{
        //    clientConnected = true;

        using (var stream = args.Socket.InputStream.AsStreamForRead())
        {
            // TODO Make sure there is data in the stream.

            // The first 4 bytes will be the size of the data containing the mesh(es).
            int datasize = ReadInt(stream);

            // Allocate a buffer to hold the data.  
            byte[] dataBuffer = new byte[datasize];

            // Read the data.
            // The data can come in chunks. 
            int readsize = 0;

            while (readsize != datasize)
            {
                readsize += stream.Read(dataBuffer, readsize, datasize - readsize);
            }

            message = $"Received {datasize}";

            //// Pass the data to the mesh serializer. 
            //List<Mesh> meshes = new List<Mesh>(SimpleMeshSerializer.Deserialize(dataBuffer));

            //if (meshes.Count > 0)
            //{
            //    // Use the network-based mapping source to receive meshes in the Unity editor.
            //    SpatialMappingManager.Instance.SetSpatialMappingSource(this);
            //}

            //// For each mesh, create a GameObject to render it.
            //for (int index = 0; index < meshes.Count; index++)
            //{
            //    int meshID = SurfaceObjects.Count;

            //    SurfaceObject surface = CreateSurfaceObject(
            //        mesh: meshes[index],
            //        objectName: "Beamed-" + meshID,
            //        parentObject: transform,
            //        meshID: meshID
            //        );

            //    surface.Object.transform.parent = SpatialMappingManager.Instance.transform;

            //    AddSurfaceObject(surface);
        }

        // Finally disconnect.

        //sender.Dispose();

        //clientConnected = false;

        //StartServer();
        //}
    }
#endif

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System;

public class NetworkInfo : MonoBehaviour
{

    public Text _pingText;
    public Text _lossText;
    public Text _fpsText;
    int amt_ping;

    // Start is called before the first frame update
    void Start()
    {
        amt_ping = 6;
    }

    // Update is called once per frame
    void Update()
    {
        ping_hosts();
        _fpsText.text = "FPS: " + 1 / Time.deltaTime;
    }

    private void ping_hosts()
    {

        try
        {
            System.Net.NetworkInformation.Ping myPing = new System.Net.NetworkInformation.Ping();

            PingReply reply = myPing.Send(Client.instance.ip, 1000);
            if (reply != null)
            {
              amt_ping = int.Parse(reply.RoundtripTime.ToString());

            }
        }
        catch
        {
            Debug.LogError("ERROR: You have Some TIMEOUT issue");
        }

        _pingText.text = "Ping: "+  amt_ping +" ms";

        // Ping the host four times
        double loss = get_loss(Client.instance.ip, amt_ping);
        _lossText.text = "Loss: " + Convert.ToString(loss) + "%";

    }

    private double get_loss(string host, int pingAmount)
    {
        System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
        PingOptions options = new PingOptions();

        // Use the default Ttl value which is 128,
        // but change the fragmentation behavior.
        options.DontFragment = true;

        // Create a buffer of 32 bytes of data to be transmitted.
        string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        int timeout = 120;
        int failed = 0;

        // Loop the amount of times to ping
        for (int i = 0; i < pingAmount; i++)
        {
            PingReply reply = pingSender.Send(host, timeout, buffer, options);
            if (reply.Status != IPStatus.Success)
            {
                failed += 1;
            }

        } // End For

        // Return the percentage
        double percent = 0;
        if (failed > 0)
        {
           percent = (failed / pingAmount) * 100;
        }
        return percent;
    }

}

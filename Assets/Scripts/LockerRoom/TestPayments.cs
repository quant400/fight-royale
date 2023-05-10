using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class TestPayments : MonoBehaviour
{
    private string squareAccessToken = "EAAAEJesuvUv7YqlgFjBxhCzoc1i1bMdlPpHNzNb7dm3nKHxrbpgjsRibKX1UWAz";

    // Replace with your own Square Application ID
    private string squareApplicationId = "YOUR_SQUARE_APPLICATION_ID";

    // Replace with your own Square Location ID
    private string squareLocationId = "LMFASYNGH5MMW";

    // Start is called before the first frame update
    void Start()
    {
        Guid myGUID = Guid.NewGuid();
        Debug.Log(myGUID.ToString());
        Debug.Log(Guid.NewGuid().ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePayment(string nonce, decimal amount)
    {
        StartCoroutine(PostPayment(nonce, amount));
    }

    private IEnumerator PostPayment(string nonce, decimal amount)
    {
        // Set up the API endpoint URL
        string url = "https://connect.squareup.com/v2/payments";

        // Set up the payment request object
        PaymentRequest paymentRequest = new PaymentRequest();
        paymentRequest.idempotency_key = System.Guid.NewGuid().ToString();
        paymentRequest.amount_money = new Money();
        paymentRequest.amount_money.amount = (int)(amount * 100); // Convert to cents
        paymentRequest.amount_money.currency = "USD";
        paymentRequest.source_id = nonce;
        paymentRequest.location_id = squareLocationId;
        paymentRequest.application_id = squareApplicationId;

        // Convert the payment request object to JSON
        string json = JsonConvert.SerializeObject(paymentRequest);

        // Set up the HTTP request headers
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", "Bearer " + squareAccessToken);
        headers.Add("Content-Type", "application/json");

        // Send the HTTP request
        UnityWebRequest www = UnityWebRequest.Post(url, json);
        foreach (KeyValuePair<string, string> header in headers)
        {
            www.SetRequestHeader(header.Key, header.Value);
        }
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error creating Square payment: " + www.error);
        }
        else
        {
            // Parse the JSON response
            PaymentResponse paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(www.downloadHandler.text);

            // Handle the response
            Debug.Log("Square payment created: " + paymentResponse.id);
        }
    }

    // Payment request object
    private class PaymentRequest
    {
        public string idempotency_key;
        public Money amount_money;
        public string source_id;
        public string location_id;
        public string application_id;
    }

    // Payment response object
    private class PaymentResponse
    {
        public string id;
        public string status;
    }

    // Money object
    private class Money
    {
        public int amount;
        public string currency;
    }
}

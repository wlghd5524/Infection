using UnityEngine;
using System.Text;
using MySql.Data.MySqlClient;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class User
{
    public int id;
    public string name;
    public string number;
}

public class MySQLConnector : MonoBehaviour
{
    private string url = "http://CGlabHospital.iptime.org/get_users.php";

    void Start()
    {
        StartCoroutine(GetUsersData());
    }

    IEnumerator GetUsersData()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            byte[] result = www.downloadHandler.data;
            string jsonResponse = Encoding.UTF8.GetString(result);
            Debug.Log("JSON Response: " + jsonResponse); // JSON 응답 확인
            User[] users = JsonHelper.FromJson<User>(jsonResponse);
            foreach (User user in users)
            {
                Debug.Log("ID: " + user.id + ", Name: " + user.name + ", Number: " + user.number);
            }
        }
    }
}
// JsonHelper class to handle array responses
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

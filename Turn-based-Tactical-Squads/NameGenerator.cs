using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameGenerator
{
    //Singleton code
    private static NameGenerator s_Instance = null;

    private List<string> m_Forenames = new List<string> {
        "John",
        "James",
        "Gary",
        "Steven",
        "Rob",
        "Nick",
        "Liam",
        "Richard",
        "Peter",
        "David",
    };

    private List<string> m_Surnames = new List<string> {
        "Jones",
        "Smith",
        "O'Hara",
        "Gibson",
        "Masters",
        "Ericson",
        "Taylor",
        "Carter",
        "Weaver",
        "Thatcher",
    };

    public static NameGenerator GetInstance()
    {
        if (s_Instance == null)
            s_Instance = new NameGenerator();

        return s_Instance;
    }

    public string GetRandomName()
    {
        return (m_Forenames[UnityEngine.Random.Range(0, m_Forenames.Count)] + " " + m_Surnames[UnityEngine.Random.Range(0, m_Surnames.Count)]);
    }
}

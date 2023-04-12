
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
public static class SeededRandom
{
    
    private static string currentSeed;    //Current Loaded Seed

    public static string GetCurrentSeed()
    {
        if(currentSeed.IsNullOrWhitespace()) GenerateRandomSeed();
        return currentSeed;
    }
    
    //Generate Random seed for the system
    public static void GenerateRandomSeed()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        currentSeed = tempSeed.ToString();

        Random.InitState(tempSeed);
    }
    public static void GenerateNewSeed(string seed = "")
    {
        currentSeed = seed;

        int tempSeed = 0;

        if (isNumeric(seed))
            tempSeed = System.Int32.Parse(seed);
        else
            tempSeed = seed.GetHashCode();

        Random.InitState(tempSeed);
    }
    public static void SetRandomSeed(int seed)
    {
        currentSeed = seed.ToString();
        int tempSeed = seed;
        Random.InitState(tempSeed);
    }

    public static int GetInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float GetFloat(float max = 1.0f)
    {
        return UnityEngine.Random.Range(0, max);
    }
    
    public static T GeElement<T>(T[] array) 
    {
        if (array == null) return default(T);
        if (array.Length == 0) return default(T);
        int i = GetInt(0, array.Length);
        return array[i];
    }
    public static T GeElement<T>(List<T> array) 
    {
        if (array == null) return default(T);
        if (array.Count == 0) return default(T);
        int i = GetInt(0, array.Count);
        return array[i];
    }

    public static List<T> Shuffle<T>(List<T> array, int shuffles = 1)
    {
        for(int s = 0; s < shuffles; s++) {
            for(int i = array.Count-1; i >= 0; i--) {
                int j = GetInt(0,array.Count);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
        return array;
    }
    //Check if Seed is All numbers
    public static bool isNumeric(string s)
    {
        return int.TryParse(s, out int n);
    }
}

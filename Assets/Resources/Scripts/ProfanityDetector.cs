using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ProfanityDetector
{
    public static bool ContainsProfanity(string text) { 

        Regex nonalpha = new Regex("[^a-zA-Z]");
        
        HashSet<string> profanitySet = new HashSet<string>();
        
        string profanityPath = "Assets/Resources/Scripts/profanity.txt"; //put in path to the profanity file:) REEEE
                
        string[] profanityList = File.ReadAllLines(profanityPath, Encoding.UTF8);
            
        foreach (string profanity in profanityList) {
            profanitySet.Add(profanity);
        }
        
        string[] words = Regex.Split(text, "[^a-zA-Z]"); //include only alpha chars
        
        foreach (string word in words) {
            if (profanitySet.Contains(word)) {
                return true;
            }
        }
        
        return false;
    }
}
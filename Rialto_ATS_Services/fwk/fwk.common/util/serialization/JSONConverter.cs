using System;
using Microsoft.VisualBasic;

namespace fwk.common.util.serialization
{
    public class JSONExtractor
    {
        public static string GetJSONKey(string key, string json)
        {
            int safetyCounter = 0;
            int index = json.IndexOf(key) + key.Length;
            
            //We go to the value after :"
            
            bool end = false;
            while (!end)
            {
                char prevChar = json[index - 1];
                char currChar = json[index];

                index++;
                safetyCounter++;

                if (safetyCounter > 1000)
                    throw new Exception(string.Format("Critical error finding key {0} in json{1}", key, json));

                if (prevChar == ':' && currChar == '"')
                    end = true;
            }

            string value = "";
            end = false;

            while (!end)
            {
                char currChar = json[index];
                
                value += currChar.ToString();

                index++;
                safetyCounter++;
                
                if (safetyCounter > 1000)
                    throw new Exception(string.Format("Critical error finding key {0} in json{1}", key, json));

                if(index<=json.Length)
                {
                    char nextChar = json[index];

                    if (currChar == '"' && (nextChar == ',' || nextChar == '}'))
                        end = true;
                }
                else
                    end = true;
            }

            return value.Substring(0, value.Length - 1);//We remove the last quote
        }
    }
}
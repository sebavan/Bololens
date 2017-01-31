using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

namespace Bololens.Memory.BuiltIn
{
    /// <summary>
    /// The base personnality helpful to quickly develop new ones.
    /// </summary>
    /// <seealso cref="Bololens.Personality.BaseBotMemory" />
    public class SettingsBotMemory : BaseBotMemory
    {
        /// <summary>
        /// Saves an object in the memory.
        /// </summary>
        /// <typeparam name="T">The type of the object to load</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public override void Save<T>(string key, T value)
        {
            string json = JsonConvert.SerializeObject(value);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads an object from the memory.
        /// </summary>
        /// <typeparam name="T">The type of the object to load</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The loaded object.
        /// </returns>
        public override T Load<T>(string key)
        {
            try
            {
                string json = PlayerPrefs.GetString(key);
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception exception)
            {
                BotDebug.LogError("SettingsBotMemory: An Error occured.");
                BotDebug.LogException(exception);
            }

            return default(T);
        }

        /// <summary>
        /// Deletes an object from the memory.
        /// </summary>
        /// <param name="key">The key.</param>
        public override void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}

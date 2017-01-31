using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Memory
{
    /// <summary>
    /// Abstraction of the different memory for the bot.
    /// </summary>
    public abstract class BaseBotMemory : MonoBehaviour
    {
        /// <summary>
        /// Initialize the bot memory.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Saves an object in the memory.
        /// </summary>
        /// <typeparam name="T">The type of the object to load</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public abstract void Save<T>(string key, T value);

        /// <summary>
        /// Loads an object from the memory.
        /// </summary>
        /// <typeparam name="T">The type of the object to load</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The loaded object.
        /// </returns>
        public abstract T Load<T>(string key);

        /// <summary>
        /// Deletes an object from the memory.
        /// </summary>
        /// <param name="key">The key.</param>
        public abstract void Delete(string key);
    }
}
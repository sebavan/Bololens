using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Core
{
    /// <summary>
    /// Base class to describe a caracteristic. 
    /// 
    /// This helps keeping consistency accross the different caracteristics available in the project
    /// </summary>
    /// <typeparam name="TypeOfCaracteristic">Interface of the caracteristic managed through this manager</typeparam>
    /// <typeparam name="TypeOfBuiltInChoices">Enum listing all the different available concrete implementation</typeparam>
    public abstract class BaseCaracteristicManager<TypeOfCaracteristic, TypeOfBuiltInChoices> : MonoBehaviour
        where TypeOfCaracteristic : MonoBehaviour
    {
        /// <summary>
        /// The chosen built in type for the caracteristic <seealso cref="TypeOfCaracteristic"/>
        /// </summary>
        public TypeOfBuiltInChoices BuiltInType;

        /// <summary>
        /// Overrides the <see cref="BuiltInType"/> with your custom caracteristic.
        /// </summary>
        public TypeOfCaracteristic CustomCaracteristic;

        /// <summary>
        /// The currently use caracteristic.
        /// </summary>
        protected TypeOfCaracteristic caracteristic;

        /// <summary>
        /// Gets the instance of the created caracteristic.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public TypeOfCaracteristic Instance
        {
            get
            {
                return caracteristic;
            }
        }

        /// <summary>
        /// Tirggers through messages when the component starts.
        /// </summary>
        void Start()
        {
            if (CustomCaracteristic == null)
            {
                CreateBuiltInCaracteristic();
            }
            else
            {
                caracteristic = CustomCaracteristic;
            }
        }

        /// <summary>
        /// Creates the caracteristi according to the chosen builtin type.
        /// </summary>
        protected abstract void CreateBuiltInCaracteristic();
    }
}

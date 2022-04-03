namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EventBusTest : MonoBehaviour
    {
        public void OnClickEvent()
        {
            Debug.Log("Raising Test Event...");
            //EventBus<JobEvent>.Raise(new JobEvent()
            //{
            //    workingStateChange = true,
            //    workingState = WorkingState.Working,
            //    promotion = true,
            //    newSalary = 60
            //});

            EventBus<DigitalPetEvent>.Raise(new DigitalPetEvent()
            {
                evolve = true,
                feedingAmount = 0,
            });
        }
    }
}

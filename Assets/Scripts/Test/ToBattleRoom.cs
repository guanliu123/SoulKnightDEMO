using System.Collections;
using System.Collections.Generic;
using cfg;
using EnumCenter;
using UnityEngine;

public class ToBattleRoom : MonoBehaviour
{
    private TriggerDetection detection;
    // Start is called before the first frame update
    void Start()
    {
        detection = GetComponent<TriggerDetection>();
        detection.AddTriggerListener(TriggerType.TriggerEnter,"Player", (obj) =>
        {
            //GameRoot.Instance.StartBattle();
            GameRoot.Instance.SwitchScene(new LevelScene());
        });
    }
}

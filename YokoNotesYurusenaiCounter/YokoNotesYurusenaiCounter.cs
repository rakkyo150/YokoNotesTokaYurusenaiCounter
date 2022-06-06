using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using YokoNotesYurusenaiCounter.Configuration;

namespace YokoNotesYurusenaiCounter
{
    public class YokoNotesYurusenaiCounter : BasicCustomCounter, INoteEventHandler
    {
        private readonly Vector3 labelOffset= new Vector3(
                PluginConfig.Instance.OffsetX, PluginConfig.Instance.OffsetY, PluginConfig.Instance.OffsetZ
                );

        private TMP_Text counter;
        
        private int yurusenaiNotesMissCount = 0;
        
        public override void CounterInit()
        {
            string defaultValue = $"{yurusenaiNotesMissCount}";

            TMP_Text label = CanvasUtility.CreateTextFromSettings(Settings, labelOffset);
            label.text = PluginConfig.Instance.LabelName;
            label.fontSize = 3f;

            TextAlignmentOptions counterAlign = TextAlignmentOptions.Top;

            counter = CanvasUtility.CreateTextFromSettings(Settings, labelOffset - Vector3.up * 0.2f);
            counter.lineSpacing = -26;
            counter.fontSize = 4f;
            counter.text = defaultValue;
            counter.alignment = counterAlign;
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (!IsItYokoNotes(data)) return;

            if (info.allIsOK) return;

            yurusenaiNotesMissCount++;
            UpdateText();
        }

        public void OnNoteMiss(NoteData data)
        {
            if (!IsItYokoNotes(data)) return;

            yurusenaiNotesMissCount++;
            UpdateText();
        }

        private void UpdateText()
        {
            counter.text = $"{yurusenaiNotesMissCount}";
        }

        private static bool IsItYokoNotes(NoteData data)
        {
            return data.cutDirection == NoteCutDirection.Left || data.cutDirection == NoteCutDirection.Right;
        }

        public override void CounterDestroy(){}
    }
}

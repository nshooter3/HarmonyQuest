namespace HarmonyQuest.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using GameManager;

    public class FmodChordInterpreter : ManageableObject
    {
        private static FmodChordInterpreter inst;
        public static FmodChordInterpreter instance
        {
            get
            {
                if (inst == null)
                {
                    inst = GameObject.FindObjectOfType<FmodChordInterpreter>();
                }
                return inst;
            }
        }

        private List<FmodNote> fmodChord = new List<FmodNote>();

        /// <summary>
        /// Any fmod markers that start with the fmodFlagChar contain chord information that we need to parse.
        /// </summary>
        private char fmodFlagChar = '~';

        /// <summary>
        /// Notes marked with fmodRootNoteChar are the root note of our chord. These notes form the foundation of the chord, and are generally more useful than the other notes.
        /// </summary>
        private char fmodRootNoteChar = 'r';

        /// <summary>
        /// A delimitter used to split the marker string into individual notes.
        /// </summary>
        private char fmodNoteDelimitterChar = ',';

        // Values that allow us to convert a note name into a midi value. For the note "F3", the F represents the note and the 3 represents the octave.
        // Using noteToMidiConversionMap, we see that F at the zeroth octave has a midi value of 5.
        // For the 3, we add the octaveOffset, then multiply it by the octaveMidiValue. (This is because octaves start at -1 on a midi keyboard). (3 + 1)*12 = 48.
        // When we add the note value (5) to the octave value (48), we get 53, which is the midi value of F3.
        private Dictionary<string, int> noteToMidiConversionMap;
        private int octaveMidiValue = 12;
        private int octaveOffset = 1;

        private FmodNote noteStruct;
        private StringBuilder myStringBuilder = new StringBuilder();

        public override void OnAwake()
        {
            if (inst == null)
            {
                inst = this;
            }
            else if (inst != this)
            {
                Destroy(gameObject);
            }

            InitMidiConversionMap();
        }

        /// <summary>
        /// Initialize our dictionary with the midi values of all our notes. This allows us to convert a note name into a midi value.
        /// There are a few duplicate values, since some differently named notes refer to the same key on a keyboard (C# and Db, for instance)
        /// </summary>
        private void InitMidiConversionMap()
        {
            noteToMidiConversionMap = new Dictionary<string, int>();

            noteToMidiConversionMap.Add("C", 0);
            noteToMidiConversionMap.Add("C#", 1);
            noteToMidiConversionMap.Add("Db", 1);
            noteToMidiConversionMap.Add("D", 2);
            noteToMidiConversionMap.Add("D#", 3);
            noteToMidiConversionMap.Add("Eb", 3);
            noteToMidiConversionMap.Add("E", 4);
            noteToMidiConversionMap.Add("F", 5);
            noteToMidiConversionMap.Add("F#", 6);
            noteToMidiConversionMap.Add("Gb", 6);
            noteToMidiConversionMap.Add("G", 7);
            noteToMidiConversionMap.Add("G#", 8);
            noteToMidiConversionMap.Add("Ab", 8);
            noteToMidiConversionMap.Add("A", 9);
            noteToMidiConversionMap.Add("A#", 10);
            noteToMidiConversionMap.Add("Bb", 10);
            noteToMidiConversionMap.Add("B", 11);
        }

        /// <summary>
        /// Check to see if the marker we've received from fmod is chord info before trying to parse it
        /// </summary>
        public bool IsFmodMarkerChordInformation(string marker)
        {
            return marker[0] == fmodFlagChar;
        }

        /// <summary>
        /// Convert the fmod chord marker string into a list of FmodNotes, then store the active notes in fmodChord.
        /// </summary>
        public void ParseChordFromMarker(string marker)
        {
            //Debug.Log("ParseChordFromMarker: " + marker);
            try
            {
                ResetFmodChord();
                
                myStringBuilder.Clear();
                myStringBuilder.Append(marker);
                myStringBuilder.Replace(" ", "");
                //Remove fmodFlagChar from the front of our marker
                myStringBuilder.Remove(0, 1);

                string[] delimitedNotesInfo = myStringBuilder.ToString().Split(fmodNoteDelimitterChar);

                for (int i = 0; i < delimitedNotesInfo.Length; i++)
                {
                    myStringBuilder.Clear();
                    myStringBuilder.Append(delimitedNotesInfo[i]);

                    noteStruct = new FmodNote();

                    if (myStringBuilder[0] == fmodRootNoteChar)
                    {
                        noteStruct.isRootNote = true;
                        //Remove fmodRootNoteChar from the start of the note when we no longer need it.
                        myStringBuilder.Remove(0, 1);
                    }
                    else
                    {
                        noteStruct.isRootNote = false;
                    }

                    //Grab the int of the end of our note name for our octave value (i.e. "C3" grabs a 3)
                    noteStruct.octave = int.Parse("" + myStringBuilder[myStringBuilder.Length - 1]);

                    //Trim octave from end of note name when we no longer need it
                    myStringBuilder.Remove(myStringBuilder.Length - 1, 1);

                    //At this point, the leftover bits of noteInfo should just be the note name.
                    noteStruct.note = myStringBuilder.ToString();

                    //Calculate our note's midi value based on the note name and the octave.
                    int midiValue = 0;
                    noteToMidiConversionMap.TryGetValue(noteStruct.note, out midiValue);
                    midiValue += (noteStruct.octave + octaveOffset) * octaveMidiValue;
                    noteStruct.midiValue = midiValue;

                    fmodChord.Add(noteStruct);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("FmodChordInterpreter error! Invalid string passed into ParseChordFromMarker: " + marker + ". Error message: " + e.Message + ", " + e.StackTrace);
            }
        }

        public void ResetFmodChord()
        {
            fmodChord.Clear();
        }

        public List<FmodNote> GetFmodChord()
        {
            return fmodChord;
        }

        public FmodNote GetFmodRootNote()
        {
            foreach (FmodNote note in fmodChord)
            {
                if (note.isRootNote)
                {
                    return note;
                }
            }
            return null;
        }

        public FmodNote GetFmodRandomNote()
        {
            return fmodChord[UnityEngine.Random.Range(0, fmodChord.Count)];
        }

        public FmodNote GetFmodNoteAtIndex(int index)
        {
            if (index < fmodChord.Count)
            {
                return fmodChord[index];
            }
            else
            {
                Debug.LogWarning("GetFmodNoteAtIndex called on index too high for current chord. Returning highest index note instead.");
                return fmodChord[fmodChord.Count - 1];
            }
        }

        public void PrintCurrentChord()
        {
            foreach (FmodNote note in fmodChord)
            {
                string messageIntro = "Note is ";
                if (note.isRootNote)
                {
                    messageIntro = "Root note is ";
                }
                Debug.Log(messageIntro + note.note + note.octave + ", midi value " + note.midiValue);
            }
        }
    }
}

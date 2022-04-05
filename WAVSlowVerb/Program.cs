using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WAVSlowVerb
{
    
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    public class Functions
    {
        public int slowRate = 1; // The default slow rate, will make no changes
        public ReaderWAV reader = new ReaderWAV(); // Creates a new instance of the ReaderWAV class, see ReaderWAV.cs for functionality
        public string path;
        public string new_path;
        public short[] og; // A checkpoint of the original audio so concurrent changes don't stack
        public void Open() // Creates a file browser window for selecting .WAV files, and reads it once one is selected
        {
            path = " ";
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = "c:\\";
            open.Filter = "wav files (*.wav)|*.wav| all files (*.*)|*.*";
            open.FilterIndex = 1;
            open.RestoreDirectory = true;

            if (open.ShowDialog() == DialogResult.OK)
            {
                path = open.FileName;
                reader.readAll(@path);
            }
            og = reader.audio;
        }
        public void Save() // Creates/Overides a file on the current path + _SlowVerb.wav, and writes the information from the reader to a new .WAV file based on the .WAV format structure (more info in the ReaderWAV class
        {
            new_path = path + "_SlowVerb.wav";
            FileStream stream = new FileStream(new_path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(reader.chunkID.ToCharArray());
            writer.Write(reader.fileSize);
            writer.Write(reader.format.ToCharArray());
            writer.Write(reader.subChunk1ID.ToCharArray());
            writer.Write(reader.subChunk1Size);
            writer.Write(reader.audioFormat);
            writer.Write(reader.numChannels);
            writer.Write(reader.sampleRate);
            writer.Write(reader.byteRate);
            writer.Write(reader.blockAlign);
            writer.Write(reader.bitsPerSample);
            writer.Write(reader.subChunk2ID.ToCharArray());
            writer.Write(reader.subChunk2Size);
            foreach (short dataPoint in reader.audio)
            {
                writer.Write(dataPoint);
            }
            writer.Seek(4, SeekOrigin.Begin);
            uint size = (uint)writer.BaseStream.Length;
            writer.Write(size - 8);
            stream.Close();
            writer.Close();
        }
        public void Slow()
        {
            // Gets the current slow rate and makes sure it's not 1
            double slowRate2 = slowRate;
            slowRate2 = slowRate2 / 10;
            bool same = (slowRate2 == 0.1) || (slowRate2 == 1);
            if (same)
            {
                slowRate2 = 1;
            }

            int newLength = Convert.ToInt32(og.Length * (2 - slowRate2)); // Gets the length of the new array based off the original increased by (slowRate)%
            short[] newData = new short[newLength]; // Makes the new array
            int y = 0; // Keeps track of when to add new data points
            int z = 0; // Keeps track of the index for the original array
            int x = 0; // Keeps track of the value of j (the current channel) before adding new data points as to continue the main loop in the correct spot
            int test = 0; // Prevents z from getting set to 0 (see more info below)
            bool add = false; // When set to true, x will overwrite the current value of j to continue the loop correctly after adding data points
            /*for (int i = 0; i < newData.Length; i++)
            {
                newData[i] = Convert.ToInt16(reader.audio[z]);
                y++;
                z++;
                if (y == 4)
                {
                    newData[i + 1] = (short)((reader.audio[z] + reader.audio[z+1]) / 2);
                    y = 0;
                    i++;
                }
            }*/
            for (int i = 0; i < newData.Length - 6; i += reader.numChannels) // Loops through all datapoints, incrementing by the number of channels each loop
            {
                for (int j = 0; j < reader.numChannels; j++) // Weaves through all channels before big loop continues
                {
                    if (add == true) // See bool add above for functionality
                    {
                        j = x;
                        add = false;
                    }
                    newData[i + j] = Convert.ToInt16(og[z]); // Sets datapoint based on channel and index of the original array
                    z++;
                    y++;
                    if (y == Convert.ToInt16((10 * slowRate2)) && same == false) //Once the loop has gone through enough to safely add 2 datapoints 
                    {
                        test = z; // For whatever reason, the value of z would get set to 1 after the completion of the next 4 lines. For now, the variable test gets set to z at the begining and overwrites z at the end to prevent this. 
                                  // If anyone knows why this is happening and could tell me that would be great and I would really appreciate it.
                        newData[i + j + 1] = (short)(((og[z - 2]) + (og[z])) / 2); // Gets the average of the original datapoint 2 spaces back and the current original datapoint, and sets it as the next data point in the new array
                        i++; // Incriments the new total datapoint count
                        newData[i + j + 1] = (short)(((og[z+1]) + (og[z-1])) / 2); // Gets the average of the next original datapoint and the previous original datapoint and sets it as the next data point in the new array
                        i++; // Incriments the new total datapoint count
                        z = test; // Sets z back to its true value
                        y = 0; // Resets the counter for adding new datapoints
                        add = true;
                        if (j == 0)
                        {
                            x = 1;
                        }
                        if (j == 1)
                        {
                            x = 0;
                        }

                    }
                }
            }
            reader.audio = newData;
            reader.subChunk2Size = (uint)(reader.audio.Length * (reader.bitsPerSample / 8)); // Gets the new size in bytes of the audio and sets it accordingly
        }
    }
}

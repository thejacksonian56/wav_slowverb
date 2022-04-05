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
        public int slowRate = 1;
        public ReaderWAV reader = new ReaderWAV();
        public string path;
        public string new_path;
        public short[] og;
        public void Open()
        {
            path = " ";
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = "c:\\";
            open.Filter = "wav files (*.wav)|*.wav| all files (*.*)|*.*";
            open.FilterIndex = 2;
            open.RestoreDirectory = true;

            if (open.ShowDialog() == DialogResult.OK)
            {
                path = open.FileName;
                reader.readAll(@path);
            }
            og = reader.audio;
        }
        public void Save()
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
            double slowRate2 = slowRate;
            slowRate2 = slowRate2 / 10;
            bool same = (slowRate2 == 0.1) || (slowRate2 == 1);
            if (same)
            {
                slowRate2 = 1;
            }
            int newLength = Convert.ToInt32(og.Length * (2 - slowRate2));
            short[] newData = new short[newLength];
            int y = 0;
            int z = 0;
            int x = 0;
            int test = 0;
            bool add = false;
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
            for (int i = 0; i < newData.Length - 6; i += reader.numChannels)
            {
                for (int j = 0; j < reader.numChannels; j++)
                {
                    if (add == true)
                    {
                        j = x;
                        add = false;
                    }
                    newData[i + j] = Convert.ToInt16(og[z]);
                    z++;
                    y++;
                    if (y == Convert.ToInt16((10 * slowRate2)) && same == false)
                    {
                        test = z;
                        newData[i + j + 1] = (short)(((og[z - 2]) + (og[z])) / 2);
                        i++;
                        newData[i + j + 1] = (short)(((og[z+1]) + (og[z-1])) / 2);
                        i++;
                        z = test;
                        y = 0;
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
            reader.subChunk2Size = (uint)(reader.audio.Length * (reader.bitsPerSample / 8));
        }
    }
}

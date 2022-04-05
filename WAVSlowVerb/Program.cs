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
        public ReaderWAV reader = new ReaderWAV();
        public string path;
        public string new_path;
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
            int newLength = Convert.ToInt32(reader.audio.Length * 1.20);
            short[] newData = new short[newLength];
            int y = 0;
            int z = 0;
            for (int i = 0; i < newData.Length; i++)
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
            }
            reader.audio = newData;
            reader.subChunk2Size = (uint)(reader.audio.Length * (reader.bitsPerSample / 8));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WAVSlowVerb
{
    /* The ReaderWAV class is designed to unpack data from a selected .WAV file and store it's data into variables which can be accessed and modified. An explanation and visual representation of how as .WAV file is formatted can be found here:
     * http://soundfile.sapp.org/doc/WaveFormat/
     * Using this information on what data is found on each specific byte, the methods in this class use a binary reader to parse through the file byte by byte and convert the data to its repective data type.*/
    public class ReaderWAV
    {
        BinaryReader reader;
        FileStream stream;
        StringBuilder builder;
        // Defines variables for header
        public string chunkID;
        public uint fileSize = 0;
        public string format;
        // Defines variables for the format chunk
        public string subChunk1ID;
        public uint subChunk1Size;
        public ushort audioFormat;
        public ushort numChannels;
        public uint sampleRate;
        public uint byteRate;
        public ushort blockAlign;
        public ushort bitsPerSample;
        // Defines variables for the data chunk (If there's a LIST subchunk, the program will overwrite it's info in favor of the data chunk)
        public string subChunk2ID;
        public uint subChunk2Size;
        public short[] audio;

        public ReaderWAV()
        {
            
        }

        // THe readHeader() method works through the header chunk
        private void readHeader()
        {
            // Defines arrays to hold individual bytes. Sections that are known to be chars are defined as char arrays and converted by byte
            char[] id = new char[4];
            byte[] size = new byte[4];
            char[] mat = new char[4];
            

            for (int i = 0; i < 4; i++) // Parses through the first 4 bytes of the file, where we know ChunkID is
            {
                id[i] = Convert.ToChar(reader.ReadByte()); // Converts each byte to char and places it in the array
                builder.Append(id[i]); // Adds the char to the string builder
            }
            chunkID = builder.ToString(); // Combines the chars to one string
            builder.Clear(); // Clears the builder for the next string

            for (int i = 0; i < 4; i++) // Parses through the next 4 bytes, where we know ChunkSize is
            {
                size[i] = reader.ReadByte(); // Adds bytes to the array
            }

            fileSize = BitConverter.ToUInt32(size, 0); // Converts the 4 bytes to an unsigned int

            for (int i = 0; i < 4; i++) // Parses through the next 4 bytesm where we know Format is
            {
                mat[i] = Convert.ToChar(reader.ReadByte()); // Converts the byte to char and adds it to an array
                builder.Append(mat[i]); // Adds the char to the string builder
            }
            format = builder.ToString(); // Combines the chars to one string
            builder.Clear(); // Clears the builder for the next string

            //Console.WriteLine("ChunkID = " + chunkID);
            //Console.WriteLine("Size = " + fileSize);
            //Console.WriteLine("Format = " + format);

        }

        // The readFormat() method works through the format chunk, similar to the method above
        private void readFormat()
        {
            // Defines arrays to hold individual bytes by section. subID is an array of chars beacause we know those bytes will be chars
            char[] subID = new char[4]; // string subChunk1ID
            byte[] subSize = new byte[4]; // uint subChunk1Size
            byte[] aFormat = new byte[2]; // ushort audioFormat
            byte[] nChannels = new byte[2]; // ushort numChannels
            byte[] sRate = new byte[4]; // uint sampleRate
            byte[] bRate = new byte[4]; // uint byteRate
            byte[] bAlign = new byte[2]; // ushort blockAlign
            byte[] bPerSample = new byte[2]; // ushort bitsPerSample

            // The method now goes through the chunk and converts/collects data in the same way as the method above, based on the knowledge of the .WAV format

            for (int i = 0; i < 4; i++)
            {
                subID[i] = Convert.ToChar(reader.ReadByte());
                builder.Append(subID[i]);
            }
            subChunk1ID = builder.ToString();
            builder.Clear();

            for (int i = 0; i < 4; i++)
            {
                subSize[i] = reader.ReadByte();
            }
            subChunk1Size = BitConverter.ToUInt16(subSize, 0);

            for (int i = 0; i < 2; i++)
            {
                aFormat[i] = reader.ReadByte();
            }
            audioFormat = BitConverter.ToUInt16(aFormat, 0);

            for (int i = 0; i < 2; i++)
            {
                nChannels[i] = reader.ReadByte();
            }
            numChannels = BitConverter.ToUInt16(nChannels, 0);

            for (int i = 0; i < 4; i++)
            {
                sRate[i] = reader.ReadByte();
            }
            sampleRate = BitConverter.ToUInt32(sRate, 0);

            for (int i = 0; i < 4; i++)
            {
                bRate[i] = reader.ReadByte();
            }
            byteRate = BitConverter.ToUInt32(bRate, 0);

            for (int i = 0; i < 2; i++)
            {
                bAlign[i] = reader.ReadByte();
            }
            blockAlign = BitConverter.ToUInt16(bAlign, 0);

            for (int i = 0; i < 2; i++)
            {
                bPerSample[i] = reader.ReadByte();
            }
            bitsPerSample = BitConverter.ToUInt16(bPerSample, 0);

            //Console.WriteLine(subChunk1ID);
            //Console.WriteLine(subChunk1Size);
            //Console.WriteLine(audioFormat);
            //Console.WriteLine(numChannels);
            //Console.WriteLine(sampleRate);
            //Console.WriteLine(byteRate);
            //Console.WriteLine(blockAlign);
            //Console.WriteLine(bitsPerSample);


        }
        // The readData() method works through the data chunk. It starts similar to the methods above, but is different once it starts the data block for reasons explained below
        private void readData()
        {
            char[] sChunkID = new char[4];
            byte[] sChunkSize = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                sChunkID[i] = Convert.ToChar(reader.ReadByte());
                builder.Append(sChunkID[i]);
            }
            subChunk2ID = builder.ToString();
            builder.Clear();

            for (int i = 0; i < 4; i++)
            {
                sChunkSize[i] = reader.ReadByte();
            }
            subChunk2Size = BitConverter.ToUInt32(sChunkSize, 0);

            // Checks to see if this chunk is a LIST subchunk. If so, the program  will skip it and procede to overwrite subChunk2 variables with the data chunk info
            if (subChunk2ID == "LIST") 
            {
                reader.ReadBytes((int)subChunk2Size);

                for (int i = 0; i < 4; i++)
                {
                    sChunkID[i] = Convert.ToChar(reader.ReadByte());
                    builder.Append(sChunkID[i]);
                }
                subChunk2ID = builder.ToString();
                builder.Clear();

                for (int i = 0; i < 4; i++)
                {
                    sChunkSize[i] = reader.ReadByte();
                }
                subChunk2Size = BitConverter.ToUInt32(sChunkSize, 0);
            }

            // Parsing through the data chunk is going to be different than the loops above. Since the size of it is variable, the method is going to create arrays after the subChunk2Size variable has been filled.
            // Once it has data, the method is going to create one array that has a length of subChunk2Size devided by 2, the number of items since we know this block of data is going to be all 16 bit integers
            short[] data = new short[subChunk2Size / 2];
            // A second array is created to act as a temporary collection of bytes to be converted to a 16 bit integer
            byte[] temp = new byte[2];

            for(int i = 0; i < data.Length; i++) //The first for loop goes until all data has been converted and collected
            {
                for(int y = 0; y < 2; y++) // The second loop goes through the chunk 2 bytes at a time, adding them to the temporary array 
                {
                    temp[y] = reader.ReadByte();
                }
                data[i] = BitConverter.ToInt16(temp, 0); // Before the loop goes through again, we convert the bytes to a 16 bit integer and store it in the data array
            }
            audio = data;

            //Console.WriteLine(subChunk2ID);
            //Console.WriteLine(subChunk2Size);


        }

        // The readAll() method executes all 3 methods above in the correct order and closes the class' BinaryReader and FileStream objects
        public void readAll(string path)
        {
            stream = new FileStream(path, FileMode.Open); // FileStream to open the selected WAV file
            reader = new BinaryReader(stream); // BinaryReader to parse through bytes in opened WAV file
            builder = new StringBuilder(); // Stringbuilder to convert all chars in an array to a string
            readHeader();
            readFormat();
            readData();
            stream.Close();
            reader.Close();
        }

    }
}

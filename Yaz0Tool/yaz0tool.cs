using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TestToo
{
    public partial class Yaz0 : Form
    {
        /*
            All Yaz0/RARC stuff adapted from C++ code written by thakis & shevious!
        */

        bool isEnc = true;
        bool isDec = false;
        bool prevFlag = false;
        int prevNumBytes = 0;
        int prevMPos = 0;

        void toOutput(String mess)
        {
            if (status.Text != "")
            {
                status.Text += ("\r\n" + mess);
            }
            else
            {
                status.Text = mess;
            }
            status.Update();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Rarc Dump Stuff
        ///////////////////////////////////////////////////////////////////////////////////////////

        const int sizeofrarch = 64; // Full sizes of the structures including unknowns
        const int sizeofnode = 16;
        const int sizeoffe = 20;

        /*
            These are the rarc structs translated from the rarcdump source.
            I've commented the unknowns, but I set up reads in the 'get' functions for future use.
        */
        struct RarcHeader
        {
            //char type[4];
            public UInt32 size;                 // Size of the RARC file.
            //UInt32 unknown;
            public UInt32 dataStartOffset;      // Offset to where the actual archive data starts.
            //UInt32 unknown2[4];

            public UInt32 numNodes;             // Number of nodes (these can be either files or subdirectories).
            //UInt32 unknown3[2];
            public UInt32 fileEntriesOffset;    // Offset to the start of the file entries.
            //UInt32 unknown4;
            public UInt32 stringTableOffset;    // Offset to the start of the string table (where file/dir names are stored).
            //UInt32 unknown5[2];
        }
        
        struct Node
        {
            //char type[4];
            public UInt32 filenameOffset;       // Offset into the string table for children; first string after offset is directory name.
            //UInt16 unknown;
            public UInt16 numFileEntries;       // Number of file entries in the node.
            public UInt32 firstFileEntryOffset; // Offset into the file entries.
        }
        
        struct FileEntry
        {
            public UInt16 id;                   // File identifier; set to 0xFFFF if it's a subdirectory.
            //UInt16 unknown;
            //UInt16 unknown2;
            public UInt16 filenameOffset;       // Offset on top of the fileNameOffset from the parent node.
            public UInt32 dataOffset;           // Offset to actual file data (on top of dataStartOffset).
            public UInt32 dataSize;             // Size of the data.
            public UInt32 zero;                 // "Seems to be always '0'".
        }

        /*
            toWORD and toDWORD were modified for the UInt16 and UInt32 types.
        */
        UInt16 toWORD(UInt16 u16)
        {
            string hs = u16.ToString("X4");
            return UInt16.Parse(
                hs.Substring(2, 2) + hs.Substring(0, 2),
                System.Globalization.NumberStyles.HexNumber
            );
        }

        UInt32 toDWORD(UInt32 u32)
        {
            string hs = u32.ToString("X8");
            return UInt32.Parse(
                hs.Substring(6, 2) + hs.Substring(4, 2) + hs.Substring(2, 2) + hs.Substring(0, 2),
                System.Globalization.NumberStyles.HexNumber
            );
        }

        string getString(int pos, BinaryReader f)
        {
            long tell = f.BaseStream.Position;
            f.BaseStream.Seek(pos, 0);
            string ret = "";
            char c;
            while ((c = f.ReadChar()) != '\0')
                ret += c;
            f.BaseStream.Position = tell;
            return ret;
        }

        Node getNode(int i, BinaryReader f)
        {
            f.BaseStream.Seek(sizeofrarch + (i * sizeofnode), 0);
            Node ret;

            f.ReadBytes(4); //type[4]
            ret.filenameOffset = toDWORD(f.ReadUInt32());
            f.ReadUInt16(); // unknown
            ret.numFileEntries = toWORD(f.ReadUInt16());
            ret.firstFileEntryOffset = toDWORD(f.ReadUInt32());

            return ret;
        }

        FileEntry getFileEntry(int i, RarcHeader h, BinaryReader f)
        {
            f.BaseStream.Seek(h.fileEntriesOffset + (i * sizeoffe) + 0x20, 0);
            FileEntry ret;

            ret.id = toWORD(f.ReadUInt16());
            f.ReadUInt16(); // unknown
            f.ReadUInt16(); // unknown2
            ret.filenameOffset = toWORD(f.ReadUInt16());
            ret.dataOffset = toDWORD(f.ReadUInt32());
            ret.dataSize = toDWORD(f.ReadUInt32());
            ret.zero = toDWORD(f.ReadUInt32());

            return ret;
        }

        void dumpNode(Node n, RarcHeader h, BinaryReader f, String path)
        {
            string nodeName = getString((int)(n.filenameOffset + h.stringTableOffset + 0x20), f); // get the name of this node.
            toOutput("Unpacking: " + nodeName + "...");
            path += "\\" + nodeName;

            DirectoryInfo dir = Directory.CreateDirectory(path);
            dir.Create();

            for (int i = 0; i < n.numFileEntries; i++)
            {
                // Get the current FileEntry
                FileEntry curr = getFileEntry((int)(n.firstFileEntryOffset + i), h, f);
                if (curr.id == 0xFFFF) // subdirectory
                {
                    if (curr.filenameOffset != 0 && curr.filenameOffset != 2) // don't go to "." and ".."
                    {
                        dumpNode(getNode((int)curr.dataOffset, f), h, f, path); // dump the node associated with curr if it's a directory.
                    }
                }
                else // file
                {
                    // If it's a file, the read the data from the offset and write it into a file with the name at the proper offset.
                    string currName = getString((int)(curr.filenameOffset + h.stringTableOffset + 0x20), f);

                    Stream nf = new FileStream(path + "\\" + currName, FileMode.Create, FileAccess.Write); // open new file stream.
                    f.BaseStream.Seek(curr.dataOffset + h.dataStartOffset, 0); // seek to start of file data.

                    // Read and write the data in 1024 byte chunks.
                    UInt32 read = 0;
                    byte[] buff = new byte[1024];
                    while (read < curr.dataSize)
                    {
                        int rAmount = Math.Min(1024, (int)(curr.dataSize - read));
                        f.Read(buff, 0, rAmount);
                        nf.Write(buff, 0, rAmount);
                        read += (UInt32)rAmount;
                    }
                    nf.Close(); // make sure to close our stream!
                }
            }
        }

        void rarcDump(String fPath, String fileName)
        {
            toOutput("!!!! ~RARC Detected~ !!!!");
            Stream strm = new FileStream(fPath, FileMode.Open);
            BinaryReader src = new BinaryReader(strm, Encoding.ASCII);

            DirectoryInfo rootDir = Directory.CreateDirectory(fPath + "_dir");
            rootDir.Create();

            // Read header
            toOutput("Reading RARC header...");
            RarcHeader h;
            src.ReadBytes(4); // 'RARC'
            h.size = toDWORD(src.ReadUInt32()); // size
            src.ReadUInt32(); // unknown
            h.dataStartOffset = toDWORD(src.ReadUInt32());
            src.ReadBytes(4 * 4); // unknown2[4]
            h.numNodes = toDWORD(src.ReadUInt32());
            src.ReadBytes(2 * 4); // unknown3[2]
            h.fileEntriesOffset = toDWORD(src.ReadUInt32());
            src.ReadUInt32(); // unknown4
            h.stringTableOffset = toDWORD(src.ReadUInt32());
            src.ReadBytes(2 * 4); // unknown5[2]
            
            // dump the root node
            Node root = getNode(0, src);
            dumpNode(root, h, src, fPath + "_dir");
            
            src.Close();
            File.Delete(fPath); // We assume that rarcDump is called from Decode(), so we have no use for the file itself.
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Yaz0 Encode Stuff
        ///////////////////////////////////////////////////////////////////////////////////////////

        byte[] fileSize(int size)
        {
            byte[] fs = new byte[4];
            byte w1 = (byte)(size & 0xFF);
            byte w2 = (byte)((size >> 8) & 0xFF);
            byte w3 = (byte)((size >> 16) & 0xFF);
            byte w4 = (byte)(size >> 24);
            fs[0] = w4; fs[1] = w3; fs[2] = w2; fs[3] = w1;
            return fs;
        }

        byte[] yaz0Header(int size)
        {
            byte[] y = new byte[16];

            y[0] = 0x59; y[1] = 0x61; y[2] = 0x7A; y[3] = 0x30;

            byte[] fs = fileSize(size);

            y[4] = fs[0]; y[5] = fs[1]; y[6] = fs[2]; y[7] = fs[3];

            for (int i = 0; i < 8; i++) // add 8 byte dummy
                y[i + 8] = 0;

            return y;
        }

        unsafe int simpleEncode(byte[] src, int size, int pos, int* pMPos)
        {
            int numBytes = 1;
            int sPos = pos - 0x1000;
            int mPos = 0;
            int j;
            
            if (sPos < 0)
                sPos = 0;

            for (int i = sPos; i < pos; i++)
            {
                for (j = 0; j < size - pos; j++)
                {
                    if (src[i + j] != src[j + pos])
                        break;
                }
                if (j > numBytes)
                {
                    numBytes = j;
                    mPos = i;
                }
            }
            *pMPos = mPos;

            if (numBytes == 2)
                numBytes = 1;

            return numBytes;
        }

        unsafe int nintendoEncode(byte[] src, int size, int pos, int* pMPos)
        {
            int numBytes = 1;
            int startPos = pos - 0x1000;
            //int mPos;
            int cpmPos;
            if (prevFlag)
            {
                *pMPos = prevMPos;
                prevFlag = false;
                return prevNumBytes;
            }

            prevFlag = false;
            numBytes = simpleEncode(src, size, pos, pMPos);

            //*pMPos = mPos;

            if (numBytes >= 3)
            {
                prevNumBytes = simpleEncode(src, size, pos + 1, &cpmPos);
                prevMPos = cpmPos;
                if (prevNumBytes >= (numBytes + 2))
                {
                    numBytes = 1;
                    prevFlag = true;
                }
            }

            return numBytes;
        }

        unsafe void Encode(byte[] src, String destPath)
        {
            int size = src.Length;  //Size of the uncompressed file
            int srcPos = 0;
            int dstPos = 0;
            int validBits = 0;
            byte[] temp = new byte[1];
            byte codeByte = 0;
            byte[] dst = new byte[24];
            int mPos = 0;
            int percent = 0;

            FileStream OP = new FileStream(destPath, FileMode.Create, FileAccess.Write);

            byte[] yaz0Head = yaz0Header(size);
            OP.Write(yaz0Head, 0, 16);

            while (srcPos < size)
            {
                int numBytes = nintendoEncode(src, size, srcPos, &mPos);

                if (numBytes < 3)
                {
                    dst[dstPos++] = src[srcPos++];
                    codeByte = (byte)(codeByte | (0x80 >> validBits));
                }
                else
                {
                    int dist = srcPos - mPos - 1;
                    byte byte1, byte2, byte3;

                    if (numBytes >= 0x12)
                    {
                        byte1 = (byte)(0 | (dist >> 8));
                        byte2 = (byte)(dist & 0xFF);
                        dst[dstPos++] = byte1;
                        dst[dstPos++] = byte2;
                        if (numBytes > 0xFF + 0x12)
                        {
                            numBytes += 0xFF + 0x12;
                        }

                        byte3 = (byte)(numBytes - 0x12);
                        dst[dstPos++] = byte3;
                    }
                    else
                    {
                        byte1 = (byte)(((numBytes - 2) << 4) | (dist >> 8));
                        byte2 = (byte)(dist & 0xFF);
                        dst[dstPos++] = byte1;
                        dst[dstPos++] = byte2;
                    }
                    srcPos += numBytes;
                }
                validBits++;
                if (validBits == 8)
                {
                    temp[0] = codeByte;
                    OP.Write(temp, 0 ,1);
                    OP.Write(dst, 0, dstPos);
                    OP.Flush();
                    codeByte = 0;
                    validBits = 0;
                    dstPos = 0;
                }
                if ((srcPos * 100) / size != percent)
                {
                    percent = (srcPos * 100) / size;
                    prog.Value = percent;
                    prog.Update();
                    Yaz0.ActiveForm.Update();
                }
            }

            if (validBits > 0)
            {
                temp[0] = codeByte;
                OP.Write(temp, 0, 1);
                OP.Write(dst, 0, dstPos);
                codeByte = 0;
                validBits = 0;
                dstPos = 0;
            }

            toOutput("Yaz0 file encoded successfully!");
            toOutput("File Encoded: " + destPath);
            toOutput("File compressed from " + size + " bytes to " + OP.Length + " bytes.");
            toOutput(size - OP.Length + " bytes saved!");
            
            OP.Close();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Yaz0 Decode Stuff
        ///////////////////////////////////////////////////////////////////////////////////////////

        public unsafe void Decode(Stream srcStream, String dstPath, String fileName)
        {
            int srcPos = 0, dstPos = 0,                 // Source and destination positions in their respective buffers
                size = (int)srcStream.Length, uncSize;  // Size of compressed and uncompressed files
            byte[] srcBuff = new byte[size], dstBuff;   // Source and destination buffers.
            
            // Read compresssed file into input buffer
            srcStream.Read(srcBuff, 0, size);
            
            // Search for the 'YAZ0' file magic. This will give us the start of the yaz0 data in the file.
            while (srcPos < size - 4 && 
                  (srcBuff[srcPos]     != 89  ||
                   srcBuff[1 + srcPos] != 97  || 
                   srcBuff[2 + srcPos] != 122 ||
                   srcBuff[3 + srcPos] != 48))
            { srcPos++; }
            
            if (srcPos >= size - 4) // Could we find the yaz0 magic in the file?
            {
                toOutput("Error: Not a valid Yaz0 file");
                return;
            }

            srcPos += 4; // adjust for magic number (4 bytes)

            // Read 4 byte uncompressed size and format correctly (DWORD)
            uncSize = (srcBuff[srcPos] << 24) | (srcBuff[srcPos + 1] << 16) | (srcBuff[srcPos + 2] << 8) | srcBuff[srcPos + 3];

            srcPos += 12; // adjust for uncompressed size (4 bytes) + unused dummy memory (8 bytes)

            dstBuff = new byte[uncSize + 0x1000]; // Create destination buffer with some extra space

            // U.I elements: output sizes, set progress bar limit
            toOutput("Compressed File Size: " + size + " Bytes");
            toOutput("Uncompressed File Size: " + uncSize + " Bytes");
            prog.Maximum = uncSize;
            prog.Value = 0;
            
            byte    codeByte = 0xFF; // The code byte tells us what to do for the next 8 cycles or "read operations".
            int     bitsLeft = 0;    // Number of bits left to be read from the code byte
            while (dstPos < uncSize)
            {
                if (bitsLeft == 0)  // We don't have any bits to read left in the code byte, so
                {                   // read the next byte in the source buffer as the new code byte
                    codeByte = srcBuff[srcPos++];
                    bitsLeft = 8;
                }

                if ((codeByte & 0x80) != 0) // If the current (leftmost) bit of the code byte is 0, then we do a 1 byte copy
                {
                    dstBuff[dstPos++] = srcBuff[srcPos++];
                }
                else // If the current bit is 1, the we decode using nintendo's RLE scheme, which means we copy a sequence (or "run")
                {    // of bytes to the front of our destination buffer from the data that we have already decoded inot dstBuff earlier.
                    byte byte1 = srcBuff[srcPos++]; // First, read 2 bytes from the source buffer; these will give us info about our run
                    byte byte2 = srcBuff[srcPos++];

                    // We use the calculation below to determine how far back we should be copying the run
                    int copyPos = dstPos - (((((byte1 & 0xF) << 8)) | byte2) + 1);

                    // # of bytes to copy: determined by the upper nibble (leftmost four bits) of byte1
                    int numBytes = byte1 >> 4;
                    if (numBytes == 0)
                        numBytes = srcBuff[srcPos++] + 0x12; // next byte from source + 0x12 (18 DEC).
                    else
                        numBytes += 2;

                    while (numBytes-- > 0) // Copy the run
                        dstBuff[dstPos++] = dstBuff[copyPos++];
                }

                // Shift the code bits to the left to set up for next operation
                codeByte <<= 1;
                bitsLeft -= 1;

                // Progress bar code:
                // We use 100 as an update interval for the progress bar to balance speed (which is hindered by frequent changes to prog) and progress bar smoothness
                if ((dstPos - prog.Value) >= 100 || prog.Value == prog.Maximum)
                {
                    prog.Value = dstPos;
                    prog.Update();
                    //Yaz0.ActiveForm.Update();
                }
            }
            toOutput("Yaz0 file decoded successfully!");
            
            // Build the path to the new decompressed file (if possible, shorten file name by removing a ".yaz0" file extension)
            String newPath = "";
            if (fileName.Substring(fileName.Length - 5, 5) == ".yaz0" && fileName[fileName.Length - 5] == '.')
            {
                newPath = dstPath + "\\" + fileName.Substring(0, fileName.Length - 5);
            }
            else
            {
                newPath = dstPath + "\\" + fileName + ".rarc";
            }

            // Write decompressed file to new path
            toOutput("Writing to: " + newPath + "...");
            FileStream OP = new FileStream(newPath, FileMode.Create, FileAccess.Write);
            OP.Write(dstBuff, 0, uncSize);
            OP.Close();
            
            // Test for RARC archive (often packed in yaz0) and if we find one, unpack it
            for (int x = 0; x < uncSize - 4; x++) {
                if (dstBuff[x]     == 0x52 && 
                    dstBuff[x + 1] == 0x41 && 
                    dstBuff[x + 2] == 0x52 && 
                    dstBuff[x + 3] == 0x43)
                {
                    rarcDump(dstPath + "\\" + fileName + ".rarc", fileName);
                    break;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Yaz0 Form Stuff
        ///////////////////////////////////////////////////////////////////////////////////////////

        public Yaz0()
        {
            InitializeComponent();
        }

        private void browse_output_Click(object sender, EventArgs e)
        {
            output.ShowDialog();
            output_path.Text = output.SelectedPath;
        }

        private void browse_input_Click(object sender, EventArgs e)
        {
            input.ShowDialog();
        }

        private void input_FileOk(object sender, CancelEventArgs e)
        {
            input_path.Text = input.FileName;
        }

        public void go_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (input_path.Text == "")
                {
                    toOutput("Please enter valid input and output paths!");
                }
                else
                {
                    if (output.SelectedPath == "") // If no output path, then we choose the input file's parent folder.
                        output.SelectedPath = input_path.Text.Substring(0, input_path.Text.LastIndexOf("\\"));

                    go_button.Enabled = false;
                    go_button.Update();

                    Stopwatch tm = new Stopwatch(); // Timer so we can tell the user how long we took (mainly for debug purposes).
                    tm.Start();

                    Stream src = input.OpenFile();

                    if (isDec)
                    {
                        toOutput("Decoding " + input.SafeFileName + "...");
                        Decode(src, output.SelectedPath, input.SafeFileName);
                    }
                    else
                    {
                        int size = (int)src.Length;
                        byte[] srcCPY = new byte[size];
                        src.Read(srcCPY, 0, size);

                        toOutput("Encoding " + input.SafeFileName + "...");
                        Encode(srcCPY, output.SelectedPath + "\\" + input.SafeFileName + ".yaz0");
                    }

                    src.Close();
					
                    tm.Stop();

                    if ((int)(tm.ElapsedMilliseconds / 1000) != 0)
                        toOutput("Operation completed in " + tm.ElapsedMilliseconds / 1000 + " seconds!");
                    else
                        toOutput("Operation completed in " + tm.ElapsedMilliseconds + " milliseconds!");

                    go_button.Enabled = true;
                    input.Reset();
                    output.Reset();
                    input_path.Text = "";
                    output_path.Text = "        Default is input file's parent folder";
                }
            }
            catch (ArgumentNullException excp)
            {
                toOutput("Please enter valid input and output paths!\r\n Exception thrown: " + excp.ToString());
            }

            return;
        }

        private void dec_Click(object sender, EventArgs e)
        {
            if (isEnc) 
			{
                isEnc = false;
                enc.Checked = false;
                isDec = true;
                dec.Checked = true;
            }
        }

        private void enc_Click(object sender, EventArgs e)
        {
            if (isDec) 
			{
                isDec = false;
                dec.Checked = false;
                isEnc = true;
                enc.Checked = true;
            }
        }

        private void clear_output_Click(object sender, EventArgs e)
        {
            status.Text = "";
            bgw.RunWorkerAsync();
            prog.Value = 0;
        }
    }
}
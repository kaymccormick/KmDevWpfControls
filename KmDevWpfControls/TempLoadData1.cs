using System;
using System.Diagnostics;
using System.IO;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    public class TempLoadData1
    {
        /// <summary>
        /// 
        /// </summary>
        public Subnode1 Node { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MemoryStream MemoryStream { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            try
            {
                return
                    $"{nameof(Node)}: {Node}, {nameof(Data)}: {Data}, {nameof(Stream)}: {Stream}, {nameof(Length)}: {Length}, {nameof(MemoryStream)}: {MemoryStream}, {nameof(Value)}: {Value}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return base.ToString();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;

namespace InkingNewsstand.Utilities
{
    public static class InkHelper
    {
        /// <summary>
        /// 序列化笔迹
        /// </summary>
        /// <param name="strokes">笔迹列表</param>
        /// <returns>序列化后的字节数组</returns>
        public static async Task<byte[]> SerializeStrokes(InkPresenter presenter)
        {
            byte[] bytes = null;

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                //先把笔迹输出到输出流
                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    await presenter.StrokeContainer.SaveAsync(outputStream);
                }

                //从输入流中读出序列化结果
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    var dataReader = new DataReader(inputStream); //在该输入流中附着一个数据读取器
                    uint loadBytes = await dataReader.LoadAsync((uint)stream.Size); //加载数据数据到中间缓冲区
                    bytes = new byte[stream.Size];
                    dataReader.ReadBytes(bytes);
                }
            }
            return bytes;
        }
    }
}

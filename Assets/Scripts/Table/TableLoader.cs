using UnityEngine;
using System;
using Newtonsoft.Json;

namespace HSMLibrary.Tables
{
    using Cysharp.Threading.Tasks;
    using HSMLibrary.Generics;
    using System.IO;

    public interface ITable
    {
        UniTask<bool> Initialize();
    }


    public class TableLoader : Singleton<TableLoader>
    {
        public async UniTask<T> LoadTableJson<T>(string _path)
        {
            var data = await LoadJson(_path);
            string json = data.text;

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async UniTask<T> LoadTableBinary<T>(string _path) where T : new()
        {
            return await BinaryConverter.Deserialize<T>(_path);
        }

        private async UniTask<TextAsset> LoadJson(string _path)
        {
            try
            {
                string path = Path.Combine("Table", $"{_path}");
                Debug.Log($"PATH : {path}");
                //.. TODO :: Addressable / ?????? ????
                var asset = await Resources.LoadAsync(path, typeof(TextAsset)) as TextAsset;

                return asset;
            }
            catch(Exception _ex)
            {
                throw new Exception($"{_ex.Message}");
            }
        }
    }
}


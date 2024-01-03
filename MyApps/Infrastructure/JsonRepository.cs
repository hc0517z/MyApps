using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyApps.Infrastructure;

public abstract class JsonRepository<T> : MemoryRepository<T> where T : Entity
{
    private readonly string _filePath;

    protected JsonRepository(string filePath)
    {
        // 프로그램 실행 파일이 있는 폴더의 경로를 가져온다.
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
        EnsureFilePath();
        Load();
    }

    private void EnsureFilePath()
    {
        if (!File.Exists(_filePath)) Save();
    }

    public override async Task<T> AddAsync(T newItem)
    {
        var item = await base.AddAsync(newItem);
        Save();
        return item;
    }

    public override async Task<T> UpdateAsync(T newItem)
    {
        var item = await base.UpdateAsync(newItem);
        Save();
        return item;
    }

    public override async Task<T> DeleteAsync(Guid id)
    {
        var item = await base.DeleteAsync(id);
        Save();
        return item;
    }

    public override async Task Clear()
    {
        await base.Clear();
        Save();
    }

    private void Load()
    {
        var json = File.ReadAllText(_filePath);
        if (string.IsNullOrEmpty(json)) return;

        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };

        var collection = JsonSerializer.Deserialize<MemoryStore<T>>(json, jsonSerializerOptions);
        _store = collection;
    }

    private void Save()
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true, Converters = { new JsonStringEnumConverter() }
        };

        var json = JsonSerializer.Serialize(_store, jsonSerializerOptions);

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

        File.WriteAllText(_filePath, json);
    }
}
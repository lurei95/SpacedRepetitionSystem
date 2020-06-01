using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Identity
{
  /// <summary>
  /// Mock implementation of <see cref="ILocalStorageService"/>
  /// </summary>
  public sealed class LocalStorageServiceMock : ILocalStorageService
  {
    ///<inheritdoc/>
    public event EventHandler<ChangingEventArgs> Changing;

    ///<inheritdoc/>
    public event EventHandler<ChangedEventArgs> Changed;

    /// <summary>
    /// The results return when calling <see cref="GetItemAsync{T}(string)"/>
    /// </summary>
    public Dictionary<string, object> GetItemResults { get; } = new Dictionary<string, object>();

    /// <summary>
    /// The results return when calling <see cref="GetItemAsync{T}(string)"/>
    /// </summary>
    public Dictionary<string, object> SetItems { get; } = new Dictionary<string, object>();

    ///<inheritdoc/>
    public Task ClearAsync()
    {
      throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public Task<bool> ContainKeyAsync(string key)
    {
      throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public Task<T> GetItemAsync<T>(string key)
    {
      if (GetItemResults.ContainsKey(key))
        return Task.FromResult((T)GetItemResults[key]);
      return Task.FromResult<T>(default);
    }

    ///<inheritdoc/>
    public Task<string> KeyAsync(int index)
    {
      throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public Task<int> LengthAsync()
    {
      throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public Task RemoveItemAsync(string key)
    {
      SetItems.Remove(key);
      return Task.FromResult<object>(null);
    }
    ///<inheritdoc/>
    public Task SetItemAsync<T>(string key, T data)
    {
      SetItems.Add(key, data);
      return Task.FromResult<object>(null);
    }
  }
}
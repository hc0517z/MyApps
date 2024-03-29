﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApps.Entities;
using MyApps.Models;
using MyApps.Repositories;

namespace MyApps.Services;

public class GroupService
{
    private readonly GroupRepository _groupRepository;

    public GroupService(GroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<IEnumerable<ObservableGroup>> GetGroupsAsync()
    {
        var groups = await _groupRepository.GetAllAsync();
        return groups.Select(g => new ObservableGroup(g));
    }
    
    public async Task<Group> AddGroupAsync(string name)
    {
        var newGroup = Group.Create(name);
        return await _groupRepository.AddAsync(newGroup);
    }
    
    public async Task<Group> UpdateGroupAsync(ObservableGroup observableGroup)
    {
        var group = await _groupRepository.GetByIdAsync(observableGroup.Id);
        group.Name = observableGroup.Name;
        return await _groupRepository.UpdateAsync(group);
    }

    public async Task<Group> DeleteGroupAsync(Guid id)
    {
        return await _groupRepository.DeleteAsync(id);
    }
}
#region copyright
// Copyright (c) 2026 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// OklchServices.cs
//
// This file is part of JumpForJoy Software's BlazorEmailBased package.
// 
// BlazorEmailBased is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// BlazorEmailBased is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with BlazorEmailBased. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased.Services;

public class OklchServices( IOptionsMonitor<SiteSettings> optionsMonitor, IWebHostEnvironment webHostEnv )
    : IHostedService, IDisposable
{
    private readonly Dictionary<int, string> _idColors = [];

    private IDisposable? _subscription;
    private double? _chromaOverride;

    public Task StartAsync( CancellationToken cancellationToken )
    {
        var globalsPath = Path.Combine( webHostEnv.WebRootPath,
                                        optionsMonitor.CurrentValue.IdColorSettings.CssGlobalsPath );

        optionsMonitor.CurrentValue.OnBackgroundColorChanged( globalsPath );

        _chromaOverride = optionsMonitor.CurrentValue.IdColorSettings.BackgroundColorChromaOverride;

        _subscription = optionsMonitor.OnChange( updated =>
        {
            var path = Path.Combine( webHostEnv.WebRootPath, updated.IdColorSettings.CssGlobalsPath );
            optionsMonitor.CurrentValue.OnBackgroundColorChanged( path );

            _chromaOverride = updated.IdColorSettings.BackgroundColorChromaOverride;

        } );

        return Task.CompletedTask;
    }

    public Task StopAsync( CancellationToken cancellationToken )
    {
        _subscription?.Dispose();
        return Task.CompletedTask;
    }

    public bool ColorsDefined => _idColors.Count > 0;

    public void GenerateIdComplements( List<int> memberIds, bool forceRegeneration = false )
    {
        // if we've never created complements, make sure we do
        if( _idColors.Count == 0 )
            forceRegeneration = true;

        // if complements exist, make sure their member ids match what
        // we were given
        if( !forceRegeneration )
        {
            var existingIds = _idColors.Select( kvp => kvp.Key ).ToHashSet();
            existingIds.ExceptWith( memberIds );

            if( existingIds.Count > 0 )
                forceRegeneration = true;
        }

        if( !forceRegeneration )
            return;

        var colors =
            optionsMonitor.CurrentValue.FirstIdComplement!.GetHexRgbComplements( memberIds.Count, _chromaOverride );

        _idColors.Clear();

        var idx = 0;

        foreach( var memberId in memberIds )
        {
            _idColors.Add( memberId, colors[ idx ] );

            idx++;
        }
    }

    public string this[ int id, bool inclHash = true ]
    {
        get
        {
            if( _idColors.TryGetValue( id, out var hexRgb ) )
                return inclHash ? hexRgb : hexRgb[ 1.. ];

            return "#000000";
        }
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}

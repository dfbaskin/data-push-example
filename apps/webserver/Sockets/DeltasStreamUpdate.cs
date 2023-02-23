using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

public record InitialDocument<T>(
    DeltasStreamType StreamType,
    string Id,
    T Initial
) where T : class;

public record PatchedDocument(
    DeltasStreamType StreamType,
    string Id,
    List<Operation> Patches
);

public record DeltasStreamUpdated(
    DeltasStreamType StreamType,
    string Id,
    string JsonDocument
)
{
    public static DeltasStreamUpdated ForInitialDocument<T>(
        DeltasStreamType streamType,
        string id,
        T doc
    ) where T : class
    {
        var initialDoc = new InitialDocument<T>(
            StreamType: streamType,
            Id: id,
            Initial: doc
        );
        return new DeltasStreamUpdated(
            StreamType: streamType,
            Id: id,
            JsonDocument: JsonSerializer.Serialize<InitialDocument<T>>(
                initialDoc,
                JsonUtils.SerializerOptions
            )
        );
    }

    public static DeltasStreamUpdated ForPatchedDocument(
        DeltasStreamType streamType,
        string id,
        JsonPatchDocument patches
    )
    {
        var patchedDoc = new PatchedDocument(
            StreamType: streamType,
            Id: id,
            Patches: patches.Operations
        );
        return new DeltasStreamUpdated(
            StreamType: streamType,
            Id: id,
            JsonDocument: JsonSerializer.Serialize<PatchedDocument>(
                patchedDoc,
                JsonUtils.SerializerOptions
            )
        );
    }
}

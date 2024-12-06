using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player; // ������ �� ������ ������
    public Tilemap floorTilemap; // Tilemap ����
    public LayerMask wallLayer; // ���� ���� (��� ��������)
    public float zPosition = -1f; // Z-���������� ��� ������

    private Bounds tilemapBounds; // ������� Tilemap ��� Gizmos

    private void Start()
    {
        // ��������� ������� Tilemap
        UpdateTilemapBounds();

        // ���������, ���������� �� ����� �� �����
        GameObject existingPlayer = GameObject.FindWithTag("Player");

        if (existingPlayer != null)
        {
            HandleExistingPlayer(existingPlayer);
        }
        else
        {
            SpawnNewPlayer();
        }
    }

    private void HandleExistingPlayer(GameObject existingPlayer)
    {
        Debug.Log("����� ��� ���������� �� �����. ���������� ��� �� ���������� �������.");
        Vector3 spawnPosition = FindValidFloorPosition();

        if (spawnPosition != Vector3.zero)
        {
            existingPlayer.transform.position = spawnPosition;
            Debug.Log($"����� ��������� �� �������: {spawnPosition}");
        }
        else
        {
            Debug.LogError("�� ������� ����� ���������� ����� ��� ������ ������.");
        }
    }

    private void SpawnNewPlayer()
    {
        Vector3 newSpawnPosition = FindValidFloorPosition();

        if (newSpawnPosition != Vector3.zero)
        {
            Instantiate(player, newSpawnPosition, Quaternion.identity);
            Debug.Log($"����� ������ �� �������: {newSpawnPosition}");
        }
        else
        {
            Debug.LogError("�� ������� ����� ���������� ����� ��� ������ ������.");
        }
    }

    private Vector3 FindValidFloorPosition()
    {
        UpdateTilemapBounds();

        BoundsInt bounds = floorTilemap.cellBounds;
        List<Vector3> validPositions = new List<Vector3>();

        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            if (IsValidFloorPosition(cellPosition))
            {
                Vector3 worldPosition = floorTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, zPosition);
                validPositions.Add(worldPosition);
            }
        }

        // ������������� ���������� �������
        VisualizeValidPositions(validPositions);

        return validPositions.Count > 0
            ? validPositions[Random.Range(0, validPositions.Count)]
            : Vector3.zero;
    }

    private bool IsValidFloorPosition(Vector3Int cellPosition)
    {
        // ���������, ���� �� ���� �� ������� �������
        if (!floorTilemap.HasTile(cellPosition)) return false;

        Vector3 worldPosition = floorTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, zPosition);

        // ���������, ����� ������� ���� �������� �� ����
        return Physics2D.OverlapCircle(worldPosition, 0.3f, wallLayer) == null;
    }

    private void VisualizeValidPositions(IEnumerable<Vector3> validPositions)
    {
        foreach (Vector3 position in validPositions)
        {
            Debug.DrawRay(position, Vector3.up * 0.3f, Color.green, 5f);
        }
    }

    private void UpdateTilemapBounds()
    {
        if (floorTilemap != null)
        {
            tilemapBounds = floorTilemap.localBounds;
        }
    }

    private void OnDrawGizmos()
    {
        if (floorTilemap != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(tilemapBounds.center, tilemapBounds.size);
        }
    }
}

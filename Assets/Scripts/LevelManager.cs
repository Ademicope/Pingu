using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; set; }

    // level spawning

    // list of pieces
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longblocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();
    public List<Piece> pieces = new List<Piece>(); // all lists in the pool

    public Piece GetPiece(PieceType pieceType, int visualIndex)
    {
        Piece piece = pieces.Find(x => x.type == pieceType && x.visualIndex == visualIndex && !x.gameObject.activeSelf);

        if (piece == null)
        {
            GameObject go = null;
            if (pieceType == PieceType.ramp)
                go = ramps[visualIndex].gameObject;
            else if (pieceType == PieceType.longblock)
                go = longblocks[visualIndex].gameObject;
            else if (pieceType == PieceType.jump)
                go = jumps[visualIndex].gameObject;
            else if (pieceType == PieceType.slide)
                go = slides[visualIndex].gameObject;

            go = Instantiate(go);
            piece = go.GetComponent<Piece>();
            pieces.Add(piece);
        }

        return piece;
    }
}

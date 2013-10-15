using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Field
{
    #region Direction enum

    public enum Direction
    {
        ToLeft,
        ToRight,
        ToTop,
        ToBottom
    };

    #endregion

    public LayerMask ChuzzleMask;

    private Vector3 _delta;
    private Vector3 _deltaTouch;
    private bool _directionChozen;
    private Vector3 _dragOrigin;
    private Chuzzle _draggable;
    private bool _isVerticalDrag;
    public List<Chuzzle> SelectedChuzzles = new List<Chuzzle>();
    public Chuzzle CurrentChuzzle;
    public Direction CurrentDirection;

    #region Events

    public event Action DragDrop;

    #endregion

    #region Event Invokators

    protected virtual void InvokeDragDrop()
    {
        var handler = DragDrop;
        if (handler != null)
        {
            Debug.Log("Drag dropped");
            handler();
        }
    }

    #endregion

    // Update is called once per frame
    public void Update(IEnumerable<Chuzzle> draggableChuzzles)
    {
        #region Drag

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            _dragOrigin = Input.mousePosition;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //   Debug.Log("is touch drag started");
                _dragOrigin = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }


            var ray = Camera.main.ScreenPointToRay(_dragOrigin);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Single.MaxValue, ChuzzleMask))
            {
                CurrentChuzzle = hit.transform.gameObject.GetComponent<Chuzzle>();
            }

            return;
        }

        // CHECK DRAG STATE (Mouse or Touch)
        if ((!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) && 0 == Input.touchCount)
        {
            DropDrag();
            return;
        }

        if (CurrentChuzzle == null)
        {
            return;
        }


        if (Input.GetMouseButton(0)) // Get Position Difference between Last and Current Touches
        {
            // MOUSE
            _delta = Input.mousePosition - _dragOrigin;

            //   Debug.Log("Drag delta");
        }
        else
        {
            if (Input.touchCount > 0)
            {
                // TOUCH
                _deltaTouch = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
                _delta = _deltaTouch - _dragOrigin;
                // Debug.Log("Drag delta TOUCH");
            }
        }

        _delta = Vector3.ClampMagnitude(_delta, CurrentChuzzle.Scale.x);


        if (!_directionChozen)
        {
            //chooze drag direction
            if (Mathf.Abs(_delta.x) < 1.5*Mathf.Abs(_delta.y) || Mathf.Abs(_delta.x) > 1.5*Mathf.Abs(_delta.y))
            {
                if (Mathf.Abs(_delta.x) < Mathf.Abs(_delta.y))
                {
                    //TODO: choose row
                    SelectedChuzzles = draggableChuzzles.Where(x => x.Current.x == CurrentChuzzle.Current.x).ToList();
                    _isVerticalDrag = true;
                }
                else
                {
                    //TODO: choose column
                    SelectedChuzzles = draggableChuzzles.Where(x => x.Current.y == CurrentChuzzle.Current.y).ToList();
                    _isVerticalDrag = false;
                }

                _directionChozen = true;
                //Debug.Log("Direction chozen. Vertical: " + _isVerticalDrag);
            }
        }

        if (_directionChozen)
        {
            if (_isVerticalDrag)
            {
                if (_delta.y > 0)
                {
                    CurrentDirection = Direction.ToTop;
                }
                else
                {
                    CurrentDirection = Direction.ToBottom;
                }
            }
            else
            {
                if (_delta.x > 0)
                {
                    CurrentDirection = Direction.ToLeft;
                }
                else
                {
                    CurrentDirection = Direction.ToRight;
                }
            }
        }

        // RESET START POINT
        _dragOrigin = Input.mousePosition;

        #endregion
    }

    public void LateUpdate(List<Cell> activeCells)
    {
        if (SelectedChuzzles.Any() && _directionChozen)
        {
            foreach (var c in SelectedChuzzles)
            {
                c.GetComponent<TeleportableEntity>().prevPosition = c.transform.localPosition;
                c.transform.localPosition += _isVerticalDrag ? new Vector3(0, _delta.y, 0) : new Vector3(_delta.x, 0, 0);

                var real = GamefieldUtility.ToRealCoordinates(c);
                var targetCell = GamefieldUtility.CellAt(activeCells, real.x, real.y);
                if (targetCell == null || targetCell.Type == CellTypes.Block || targetCell.IsTemporary)
                {
                    // Debug.Log("Teleport from " + currentCell);
                    switch (CurrentDirection)
                    {
                        case Field.Direction.ToRight:
                            //if border
                            if (targetCell == null)
                            {
                                targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Max(x => x.x), c.Current.y);
                                if (targetCell.Type == CellTypes.Block)
                                {
                                    targetCell = targetCell.GetLeftWithType();
                                }
                            }
                            else
                            {
                                //if block
                                targetCell = targetCell.GetLeftWithType();

                                if (targetCell == null)
                                {
                                    targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Max(x => x.x),
                                        c.Current.y);
                                    if (targetCell.Type == CellTypes.Block)
                                    {
                                        targetCell = targetCell.GetLeftWithType();
                                    }
                                }
                            }
                            break;
                        case Field.Direction.ToLeft:
                            //if border
                            if (targetCell == null)
                            {
                                targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Min(x => x.x), c.Current.y);
                                if (targetCell.Type == CellTypes.Block)
                                {
                                    targetCell = targetCell.GetRightWithType();
                                }
                            }
                            else
                            {
                                targetCell = targetCell.GetRightWithType();
                                if (targetCell == null)
                                {
                                    targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Min(x => x.x),
                                        c.Current.y);
                                    if (targetCell.Type == CellTypes.Block)
                                    {
                                        targetCell = targetCell.GetRightWithType();
                                    }
                                }
                            }
                            break;
                        case Field.Direction.ToTop:
                            //if border
                            if (targetCell == null || targetCell.IsTemporary)
                            {
                                targetCell = GamefieldUtility.CellAt(activeCells, c.Current.x,
                                    activeCells.Where(x => !x.IsTemporary).Min(x => x.y));
                                if (targetCell.Type == CellTypes.Block)
                                {
                                    targetCell = targetCell.GetTopWithType();
                                }
                            }
                            else
                            {
                                targetCell = targetCell.GetTopWithType();

                                if (targetCell == null)
                                {
                                    targetCell = GamefieldUtility.CellAt(activeCells, c.Current.x,
                                        activeCells.Where(x => !x.IsTemporary).Min(x => x.y));
                                    if (targetCell.Type == CellTypes.Block)
                                    {
                                        targetCell = targetCell.GetTopWithType();
                                    }
                                }
                            }
                            break;
                        case Field.Direction.ToBottom:
                            //if border
                            if (targetCell == null)
                            {
                                targetCell = GamefieldUtility.CellAt(activeCells, c.Current.x,
                                    activeCells.Where(x => !x.IsTemporary).Max(x => x.y));
                                if (targetCell.Type == CellTypes.Block)
                                {
                                    targetCell = targetCell.GetBottomWithType();
                                }
                            }
                            else
                            {
                                targetCell = targetCell.GetBottomWithType();

                                if (targetCell == null)
                                {
                                    targetCell = GamefieldUtility.CellAt(activeCells, c.Current.x,
                                        activeCells.Where(x => !x.IsTemporary).Max(x => x.y));
                                    if (targetCell.Type == CellTypes.Block)
                                    {
                                        targetCell = targetCell.GetBottomWithType();
                                    }
                                }
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Current direction can not be shit");
                    }
                    //  Debug.Log("Teleport to " + targetCell);

                    var difference = c.transform.localPosition -
                                     GamefieldUtility.ConvertXYToPosition(real.x, real.y, c.Scale);
                    c.transform.localPosition =
                        GamefieldUtility.ConvertXYToPosition(targetCell.x, targetCell.y, c.Scale) +
                        difference;
                }
            }
        }
    }

    private void DropDrag()
    {
        if (SelectedChuzzles.Any())
        {
            InvokeDragDrop();
            Reset();
        }
    }

    public void Reset()
    {
        SelectedChuzzles.Clear();
        CurrentChuzzle = null;
        _directionChozen = false;
        _isVerticalDrag = false;
    }
}
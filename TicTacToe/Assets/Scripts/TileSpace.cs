using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TileSpace : MonoBehaviour
{
    private int _touchID;
    public Player occupyingPlayer;

    private Sprite _occupyingIcon;

    private SpriteRenderer _renderer;

    private TaskManager _tm = new TaskManager();

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _occupyingIcon = _renderer.sprite;

        _touchID = -1;

        Services.EventManager.Register<TouchDown>(OnTouchDown);
        Services.EventManager.Register<MouseDown>(OnMouseDown);
    }

    private bool PointContainedInTile(Vector3 point)
    {
        Vector3 extents = _renderer.bounds.extents;
        Vector3 centerPoint = _renderer.transform.position;

        return  point.x >= centerPoint.x - extents.x && point.x <= centerPoint.x + extents.x &&
                point.y >= centerPoint.y - extents.y && point.y <= centerPoint.y + extents.y;
    }

    public void OnTouchDown(TouchDown e)
    {
        Vector3 touchWorldPos = 
                    Services.GameManager.MainCamera.ScreenToWorldPoint(
                                                        new Vector3( 
                                                                e.touch.position.x,
                                                                e.touch.position.y,
                                                                -Services.GameManager.MainCamera.transform.position.z));
        if(PointContainedInTile(touchWorldPos) && _touchID == -1)
        {
            OnInputDown();
        }
        
    }

    public void OnMouseDown(MouseDown e)
    {
        Vector3 mouseWorldPos = 
                    Services.GameManager.MainCamera.ScreenToWorldPoint(
                                                        new Vector3( 
                                                                e.mousePos.x,
                                                                e.mousePos.y,
                                                                -Services.GameManager.MainCamera.transform.position.z  ));
        if(PointContainedInTile(mouseWorldPos))
        {
            OnInputDown();
        }
    }

    private void OnInputDown()
    {

        Debug.Log(name + " Touch Down!");
        // Check if occupying piece is null and provide user feedback

        Services.EventManager.Register<TouchUp>(OnTouchUp);
        Services.EventManager.Register<MouseUp>(OnMouseUp);

        Services.EventManager.Unregister<TouchDown>(OnTouchDown);
        Services.EventManager.Unregister<MouseDown>(OnMouseDown);
    }

     public void OnTouchUp(TouchUp e)
    {
        if(e.touch.fingerId == _touchID)
        {
            OnInputUp();
            _touchID = -1;
        }   
    }

     public void OnMouseUp(MouseUp e)
    {
        OnInputUp();
    }

    private void OnInputUp()
    {

        //Debug.Log(name + " Touch Up!");

        
        Services.EventManager.Register<TouchDown>(OnTouchDown);
        Services.EventManager.Register<MouseDown>(OnMouseDown);

        Services.EventManager.Unregister<TouchUp>(OnTouchUp);
        Services.EventManager.Unregister<MouseUp>(OnMouseUp);
    }

    public void SetOccupyingPlayer(Player p){
        occupyingPlayer = p;
        _occupyingIcon = occupyingPlayer.PlayerIcon;
    }

    // TODO: Animation on being selected


    // Update is called once per frame
    void Update()
    {
        _tm.Update();
    }
}

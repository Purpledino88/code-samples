using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Class to handle a bouncing ball which can be destroyed on press.
Starting velocity and size will change based on game difficulty.
Difficulty is provided on initiation by BouncingBallsManager (starts on today's set difficulty but will increase as the game goes on).
Balls will decrease in size and change colour as they age.
A ball which ages enough will end the game.
*/
public class BallButton : MonoBehaviour
{
    //Various Unity components required for each ball
    private Rigidbody2D m_RigidBody;
    private Transform m_Transform;
    private SpriteRenderer m_Renderer;

    //The lifetime of the ball in seconds, starting from 5 seconds and decreasing on update
    private float m_MaximumLifeTime = 5.0f;
    private float m_CurrentLifeTime;
    public bool IsActive() { return (m_CurrentLifeTime > 0); }

    //A linear interpolation from 0-1 of the lifetime of the ball above
    private float m_Lifecycle;

    //The initial size of the ball, stored on creation
    private Vector3 m_InitialScale;

    //Boolean to stop Physics calculations and movement on game end
    private bool m_IsFrozen = false;

    /*
    Simple function called on game end to halt velocity and stop being updated.
    */
    public void Freeze() 
    { 
        m_IsFrozen = true;
        m_RigidBody.velocity = Vector2.zero;
    }

    /*
    'Constructor' for our ball.
    BouncingBallsManager will handle Unity prefab instantiation, this is only for setting up this script's start requirements
    BouncingBallsManager will also supply the current difficulty as a parameter.
    */
    public void Initiate(float difficulty_multiplier)
    {
        //Get needed components
        m_RigidBody = gameObject.GetComponent<Rigidbody2D>();
        m_Transform = gameObject.GetComponent<Transform>();
        m_Renderer = gameObject.GetComponent<SpriteRenderer>();

        //On error, delete this ball immediately (should never happen based on current prefab)
        if ((m_RigidBody == null) || (m_Transform == null) || (m_Renderer == null))
        {
            Destroy(gameObject);
        }
        else
        {   
            //Set the starting lifetime and colour
            m_CurrentLifeTime = m_MaximumLifeTime;
            m_Renderer.color = Color.green;

            //Apply random velocity to ball based on current difficulty
            m_RigidBody.velocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * difficulty_multiplier;

            //Set and save the starting size of the ball based on current difficulty
            m_InitialScale = m_Transform.localScale / difficulty_multiplier;
            m_Transform.localScale = m_InitialScale;
        }
    }

    /*
    Update loop for each ball, handling this script's requirements. Physics, rendering, etc are handled by Unity.
    */
    void Update()
    {
        if (!m_IsFrozen) //If game has not ended
        {
            m_CurrentLifeTime -= Time.deltaTime; //decrease the current lifetime

            m_Lifecycle = m_CurrentLifeTime / m_MaximumLifeTime; //Get the 0-1 lifecycle

            if (m_CurrentLifeTime > 0.0f)
            {
                //Update colour and size based on the 0-1 lifecycle
                m_Renderer.color = new Color(Mathf.Max(0f, 1.0f - m_Lifecycle), Mathf.Min(1f, m_Lifecycle), 0.0f); //As lifecycle increases, less green and more red
                m_Transform.localScale = new Vector3(m_InitialScale.x, m_InitialScale.y, m_InitialScale.z) * (0.5f + (m_Lifecycle / 2f)); //Minimum size is half the starting size
            }
        }        
    }
}
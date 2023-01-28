using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
The specific game manager for the 'Bouncing Balls' dexterity survival game.
This game features a series of balls spawning at increasing frequency which bounce around the screen.
Ball size will decrease and velocity will increase as the game goes on.
Players must click on each ball before it expires to continue playing, allowing a ball to expire ends the game.
Players must survive a certain length of time to achieve stars.
*/
public class BouncingBallsManager : SpecificManager
{
    //Required objects from the Unity editor
    public GameObject ballPrefab;
    public Transform topBound;
    public Transform bottomBound;
    public Transform leftBound;
    public Transform rightBound;

    //Constant values
    private const float BASE_NEW_BALL_INTERVAL = 4f; //Base time between each ball spawning in seconds
    private const float MAX_TIME = 60f; //Time to survive to achieve a gold star in seconds

    //List of all currently active balls
    private List<BallButton> m_Balls;

    //Paired vectors to hold the bounds of the screen
    private Vector2 m_XBounds;
    private Vector2 m_YBounds;

    //Superclass will hold today's difficulty, this variable will hold the increasing difficulty throughout this playthrough
    private float m_AdaptiveDifficulty;

    //Countdown in seconds until new ball is spawned
    private float m_NewBallCountdown = 0.0f;

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will load a number of neccessary words for the game.
    */
    protected override void LoadVocabulary()
    {
        //This game will not have vocabulary, purely dexterity
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will allow games to perform any specific tasks prior to the start of the game.
    */
    protected override void SetupGame()
    {
        m_Balls = new List<BallButton>(); //Initialise list

        //Set starting difficulty to today's base difficulty
        m_AdaptiveDifficulty = m_Difficulty;

        //Set the bounds of the area which the balls can travel in
        //TODO: Change location of bounding physics objects based on resolution of device being used (rather than the Unity editor start locations which are set for Michael's phone)
        Transform ball_tf = ballPrefab.GetComponent<Transform>();
        float ball_max_radius = Mathf.Max(ball_tf.localScale.x, ball_tf.localScale.y) / 2.0f;
        m_XBounds = new Vector2(leftBound.position.x + leftBound.localScale.x + ball_max_radius, rightBound.position.x - rightBound.localScale.x - ball_max_radius);
        m_YBounds = new Vector2(bottomBound.position.y + bottomBound.localScale.y + ball_max_radius, topBound.position.y - topBound.localScale.y - ball_max_radius);
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the game's setup phase, this function will force games to set specific reward requirements and starting state.
    */
    protected override void SetThresholds()
    {
        // Simple rewards setup based on surviving through thirds of the max time.
        // This is a timed survival game, so set progress as 0 and increase with time.
        progressPanel.SetStartValues(0.0f, 0.3333333333f, 0.6666666666f, 1.0f);
    }

    /*
    ABSTRACT FROM SUPERCLASS: Called in the update loop, this will handle the specific games frame-by-frame needs.
    */
    protected override void UpdateGame(float dt)
    {
        // Convert frame duration into 0-1 value based on the maximum time of the game and update the progress panel
        if (!progressPanel.UpdateProgress(dt / MAX_TIME)) //if progress panel says game is not valid anymore because progress has reached 1 (read: time has exceeded maxtime)
        {   
            //Freeze all the balls to stop physics and call superclass to end the game
            foreach (BallButton ball in m_Balls)
            {
                ball.Freeze();
            }
            EndGame();
        }
        else //If progress has not reached 1 (read: time has not reached the maximum)
        {
            m_AdaptiveDifficulty += dt; //Increase this instances difficulty

            //Check if any of the balls have expired
            bool game_over = false;
            foreach (BallButton ball in m_Balls)
            {
                if (!ball.IsActive())
                    game_over = true;
            }

            if (game_over)
            {
                //Freeze all the balls to stop physics and call superclass to end the game
                foreach (BallButton ball in m_Balls)
                {
                    ball.Freeze();
                }
                EndGame();
            }

            //On click/press
            if (Input.GetMouseButtonDown(0))
            {
                //Get position of press
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                //Raycast to detect if any balls are in the clicked position, and destroy them with an audio effect if they are
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    BallButton ball = hit.collider.gameObject.GetComponent<BallButton>();
                    if (ball != null)
                    {
                        m_Balls.Remove(ball);
                        Destroy(ball.gameObject);
                        m_AudioSource.PlayOneShot(m_CorrectAudio);
                    }
                }
            }

            //Decrease the countdown to spawn by the frame length and spawn a new ball if needed
            m_NewBallCountdown -= dt;
            if (m_NewBallCountdown < 0f)
            {
                AddNewBall();

                //Lower the countdown to the next spawn based on the current game instance's difficulty
                m_NewBallCountdown = (BASE_NEW_BALL_INTERVAL / (1 + (m_AdaptiveDifficulty / MAX_TIME))); 
            }
        }        
    }

    /*
    Simple function to instantiate a new ball from the prefab
    */
    private void AddNewBall()
    {
        GameObject newObj = Instantiate(ballPrefab, new Vector3(Random.Range(m_XBounds.x, m_XBounds.y), Random.Range(m_YBounds.x, m_YBounds.y), 0), Quaternion.identity);
        BallButton newBall = newObj.GetComponent<BallButton>();
        if (newBall != null)
        {
            //Size/speed of the new ball vary based on current difficulty
            newBall.Initiate(1 + (m_AdaptiveDifficulty / MAX_TIME));
            m_Balls.Add(newBall); //Add to list to be checked
        }
        else
        {
            Destroy(newObj);
        }
    }
}

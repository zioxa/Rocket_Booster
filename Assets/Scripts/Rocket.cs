using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    //this will allow to change the script from the inspector while game play mode only after that back to normal
    [SerializeField] float rcsThrust = 100f;     //Reaction Control System (rcs)

    //public float rcsThrust = 100f;   //this will allow to change the script from the inspector or other scripts permanent


    [SerializeField] float mainThrust = 100f;

    [SerializeField] float levelLoadDelay = 0.5f;

    [SerializeField] AudioClip mainEngineAudio;
    [SerializeField] AudioClip SuccessAudio;
    [SerializeField] AudioClip DeathAudio;

    [SerializeField] ParticleSystem mainEngineParticleSystem;
    [SerializeField] ParticleSystem SuccessParticleSystem;
    [SerializeField] ParticleSystem DeathParticleSystem;

    Rigidbody rigidBody;
    AudioSource audioSource;
    

    enum State { Alive, Dying, Transcending};
    State state;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        state = State.Alive;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Alive)
        {
            RespondToRotateInput();
            RespondToThrustInput();
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.PlayOneShot(SuccessAudio);
                SuccessParticleSystem.Play();
                Invoke("LoadNextLevel", levelLoadDelay);     //this Invoke method will run ChangeNextScene after levelLoadDelay second
                break;
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(DeathAudio);
                DeathParticleSystem.Play();
                Invoke("LoadFirstLevel", levelLoadDelay);
                break;

        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void RespondToRotateInput()
    {

        rigidBody.freezeRotation = true;   // take manual control of the rotaion

        float rotationThisFrame = rcsThrust * Time.deltaTime;


        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;  //resume physics control of the rotation
    }

    private void RespondToThrustInput()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
            mainEngineParticleSystem.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);

        if (audioSource.isPlaying == false)  //so it does't layer the audio in consecutive frame
        {
            audioSource.PlayOneShot(mainEngineAudio);
        }
        if (mainEngineParticleSystem.isPlaying == false)   //fix the issue of the particle system
        {
            mainEngineParticleSystem.Play();   
        }
    }
}

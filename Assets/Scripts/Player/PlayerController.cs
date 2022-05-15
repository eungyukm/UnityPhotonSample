using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private Movement _movement;
    private CharacterController _characterController;
    public AttackReader _attackReader;
    private PlayerAnimator _playerAnimator;

    private void OnEnable()
    {
        _attackReader.OnDefaultAttack += DefaultAttack;
    }

    private void OnDisable()
    {
        _attackReader.OnDefaultAttack -= DefaultAttack;
    }

    // Start is called before the first frame update
    void Start()
    {
        _movement = GetComponent<Movement>();
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            _characterController.Move(_movement.MoveDirecton * _movement.MoveSpeed * Time.deltaTime);
            _playerAnimator.OnMovement(_movement._inputX, _movement._inputZ);
            transform.rotation = Quaternion.Euler(0, _movement._cameraTransform.eulerAngles.y, 0);
        }
    }

    void DefaultAttack()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Default Attack Call!!");
            _playerAnimator.OnDefaultAttack();
        }
    }

    public override void OnLeftRoom()
    {
        // 룸을 나가면 로비 씬으로 돌아갑니다.
        SceneManager.LoadScene("GameLobby");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Gun : MonoBehaviourPun, IPunObservable
{//                   { 발사 준비, 탄창이 빔, 재장전 중 }
    public enum State { Ready, Empty, Reloading }

    public State state { get; private set; }
    public Transform fireTransform;  // 탄알 발사 위치
    public ParticleSystem muzzleFlashEffect; // 총구화염 효과
    public ParticleSystem shellEjectEffect;  // 탄피 배출 효과
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private AudioSource gunAudioPlayer;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public float damage = 25f;   // 공격력
    private float fireDistance = 50f; // 사거리
    public int ammoRemain = 100;  // 남은 전체 탄알
    public int magCapacity = 25;  // 탄창 용량
    public int magAmmo;           // 현재 탄창에 남아 있는 탄알
    public float timeBetFire = 0.12f; // 탄알 발사 간격
    public float reloadTime = 1.8f;   // 재장전 소요 시간
    private float lastFireTime; // 총을 마지막으로 발사한 시점
    public Text bulletText;

    void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();
        // 사용 할 점을 두개로 변경
        //lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        bulletText = GameObject.Find("HUD Canvas").transform.GetChild(0).
            GetChild(0).GetComponent<Text>();
    }

    private void OnEnable()
    {
        magAmmo = magCapacity; // 남은 탄창 = 탄창 용량
        state = State.Ready;
        lastFireTime = 0f;
    }

    public void Fire() // 발사 시도
    {
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();
        }
    }

    private void Shot()  // 실제 발사 처리
    {
        RaycastHit hit;
        Ray ray = new Ray(fireTransform.position, fireTransform.forward); //Ray 사용시
        Vector3 hitPosition = Vector3.zero;
        if (Physics.Raycast(ray, out hit, fireDistance)) //Ray 사용시
            //레이가 다른 물체와 충돌 했다면
                         // (총알 발사위치, 총알이 나아가는 방향, 레이, 사거리)
        //if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
           // 충돌한 상대방으로 부터 IDamageable 오브젝트 가져오기를 시도한다.
           IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                // 상대방의 OnDamage 함수를 실행 시켜 상대방에 대미지 주기
                target.OnDamage(damage, hit.point, hit.normal);
            }
            // 레이가 충돌한 위치 저장
            hitPosition = hit.point;
        }
        else // 레이가 다른 물체와 충돌 하지 않았다면
        {
            // 탄알이 최대 사정 거리까지 날아갔을 때의 위치를 충돌 위치로 사용
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
            lineRenderer.SetPosition(1, ray.GetPoint(50f));
        }
        StartCoroutine(ShotEffect(hitPosition));
        photonView.RPC("ShotProcessOnSeve", RpcTarget.MasterClient);
        // 실제 발사처리는 호스트에 대리
        magAmmo--;
        if (magAmmo <= 0)
        {
            state = State.Empty;
        }
    }

    [PunRPC]  // 호스트에서 실제로 발사 처리
    private void ShotProcessOnSever()
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
            }
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }
        photonView.RPC("ShotEffectProcessOnClient", RpcTarget.All, hitPosition);
    }

    [PunRPC]
    void ShotEffectProcessOnClient(Vector3 hitpos)
    {
        StartCoroutine(ShotEffect(hitpos));
    }

    IEnumerator ShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();  // 총기 화염 이펙트. 플레이
        shellEjectEffect.Play();   // 탄피 배출 이펙트. 플레이
        gunAudioPlayer.PlayOneShot(shotClip); // 총 발사 소리. 한번 플레이
        // 선의 시작점은 총구의 위치
        lineRenderer.SetPosition(0, fireTransform.position);
        // 선의 끝점은 입력으로 들어온 충돌 위치
        lineRenderer.SetPosition(1, hitPosition);
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.03f);
        lineRenderer.enabled = false;
    }

    public bool Reload()  // 재장전 시도
    {//               이미 재장전 중 || 남은 탄이 없다  || 탄이 이미 가득 찬 경우
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity)
        {
            return false;
        }
        StartCoroutine(ReloadRoutine());
        return true;
    }

    IEnumerator ReloadRoutine()  // 실제 재장전 처리를 진행
    {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(reloadClip);
        yield return new WaitForSeconds(reloadTime);
        // 탄창에 채울 탄알을 계산
        int ammoToFill = magCapacity - magAmmo;
        // 탄창에 채워야할 탄알이 남은 탄알보다 많다면 채워야 할 탄알 수를 남은 탄알수에 맞게 줄임
        if (ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }
        // 탄창을 채움
        magAmmo += ammoToFill;
        // 남은 탄알에서 탄창에 채운만큼 탄을 뺌
        ammoRemain -= ammoToFill;
        state = State.Ready;
    }

    public void UpdateBulletText()
    {
        bulletText.text = string.Format("{0}/{1}", magAmmo, ammoRemain);
    }

    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemain += ammo;
    }

    // 주기적으로 자동 실행 되는 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //
    {
        if (stream.IsWriting)  // 송신 로컬의 움직임 회전 이동 등
        {
            stream.SendNext(ammoRemain);
            // 남은 탄알 수를 네트워크로 통해 보내고(송신)
            stream.SendNext(magAmmo);
            // 탄창의 총알 수의 상태를 네트워크로 통해 송신
            stream.SendNext(state);
            // 현재 총의 상태를 네트워크로 보내기
        }
        else if (stream.IsReading)  // 다른 네트워크 유저의 총의 모든 상태를 수신 받는다.
        {
            ammoRemain = (int)stream.ReceiveNext();
            magAmmo = (int)stream.ReceiveNext();
            state = (State)stream.ReceiveNext();
        }
    }
}

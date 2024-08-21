using UnityEngine;

public interface IDamageable
{//    인터페이스 자체를 느슨한 커플링이다.
    // 이 메서드를 받는 클래스가 어떤 클래스인지 검사를 하지 않는다.
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}

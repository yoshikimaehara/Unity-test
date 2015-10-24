using UnityEngine;
using System.Collections;

/// <summary>
/// キャラクターユニットのインターフェースを定義する。
/// </summary>
public interface IAutohr
{

    /// <summary>
    /// キャラクターを移動させる。
    /// </summary>
    /// <param name="direction">移動する方向を指定する。</param>
    /// <returns>正常に移動処理が終了場合はtrueを返す。それ以外はfalseを返す。</returns>
    bool AttemptMove(int direction);
    
    /// <summary>
    /// キャラクターを複数マス移動させる。
    /// </summary>
    /// <param name="direction">移動する方向を指定する。</param>
    /// <param name="distance">移動する距離を指定する。</param>
    /// <returns>正常に移動処理が終了場合はtrueを返す。それ以外はfalseを返す。</returns>
    bool AttemptMove(int direction,int distance);

    /// <summary>
    /// スキルを使用する。
    /// </summary>
    /// <param name="skillType">使用するスキルの種類を指定する。</param>
    /// <returns>正常にスキル処理が終了した場合は、trueを返す。それ以外はfalseを返す。</returns>
    bool UseSkill(int skillType);

    /// <summary>
    /// 罠にかかった際の処理を実行する。
    /// </summary>
    /// <param name="trapType">罠の種類を指定する。</param>
    /// <returns>正常に罠にかかった場合は、trueを返す。それ以外はfalseを返す。</returns>
    bool AccordingTrap(int trapType);

    /// <summary>
    /// レベルアップした際のステータスを変更する。
    /// </summary>
    void LevelUp();
}
# 协防原型 · README

一个 1–4 人合作协防游戏的原型工程。核心创新是**把合作做成一条会被敌人攻击的物流链**，而不是共享血条或擦屁股式联防。当前阶段：**单人本地 + 1 机器人**，验证核心循环是否好玩。

---

## 当前目标

用最短路径跑通以下闭环，验证手感：

```
休整期（建设/抽卡/升本） → 开展期（防守/传递/拦截/合作） → 结算 → 下一轮
```

**不追求**：平衡性、美术、音效、联机、减法地图、大 Boss、评级称号。
**追求**：体力经济是否成立、传递/拦截/三方配合是否有趣、机器人是否可读。

---

## 技术栈

| 项 | 选型 |
|---|---|
| 引擎 | Unity 6.3 LTS |
| 语言 | C#（全部逻辑） |
| UI | uGUI（Canvas + Text/Button/Image） |
| 单位表现 | 2D 方块或简单 Sprite，Animator 驱动冷却表现 |
| 数据 | JSON 文件（`Application.persistentDataPath`） |
| 网络 | 不实现，接口留空 |
| 第三方库 | 无 |

---

## 原型范围

### 做

- 1 真人 + 1 机器人
- 休整期 60s / 开展期 60s 循环
- 体力系统（建设与合作共用一条体力）
- 抽卡（一页 3 张、多选、单独付费、开展期保留）
- 升本（降卡价、提选项上限）
- 相邻传递 + 嘲讽词条拦截 + 三方配合（清路/反拦截/加速）
- 小 Boss 从入口进入、逃出扣耐久、击杀给赏金
- 数据表单采集 + 结算页显示
- 机器人最简版（防守 + 概率响应合作）

### 不做

- 联机、网络同步
- 减法地图（拓扑变化）——留接口，第一版固定 2 人线段拓扑
- 大 Boss、毒圈、大水晶
- 评级/称号（只采集数据）
- 美术、音效、动画细节
- 平衡性调优（数值全用占位常量）

---

## 核心规则摘要

### 时间结构

- **休整期 60s**：体力回复快，建造/抽卡/升本/部署。
- **开展期 60s**：体力缓慢回复，敌人涌入，合作传递开放。
- 资源抽取按**个人时间轴**，不受队友影响。

### 体力

全局唯一合作/建设成本。部署、移动、建造、加工、小人合作手操都消耗体力。自己领地内小人移动不耗体力只耗时间；跨领地合作手操耗体力，有搬运上限，耗尽自动返回，被攻击减体力。

### 抽卡与升本

- 抽卡：一页 N 张（基础 3，上限 6），**多选，每张单独付费**，不选不扣费，抽取卡每轮战斗结束消失，开展期面板仍可访问。
- 升本：耗升本经验（代币购买 + 打精英/反拦截掉落），降低卡价、提升抽卡页选项上限。

### 传递与拦截

- 只能相邻传。运输单位沿路径移动，敌人优先攻击运输单位（确定性拦截）。
- 合作发生时触发**嘲讽词条敌人**，从发起方和路径领地集中攻击运输单位。
- 三方配合：中继方清路（获双倍赏金 + 中继经验）、发起方反拦截、接收方加速/给报酬。
- 中继经验：吃到悬赏的中间玩家代币消耗减少（能力成长，不是资源转移）。
- 小人拜访别人领地默认只读，**不触发嘲讽怪**；只有主动发起合作才招来拦截。

### 小 Boss

指定轮次出现，从入口进入。逃出扣小水晶耐久（跟小怪同量级），击杀给赏金 + 升本经验。

### 机器人

最简版：自己领地濒危 → 防守；有合作请求且体力 > 50% → 50% 概率响应；否则自己发展。不做议价、不做组合技权重。

---

## 目录结构

```
/src
  /core
    Config.cs            // 所有占位常量集中在这里
    GameLoop.cs          // 状态机：休整/开展/结算/小Boss
    GameState.cs         // 运行时数据容器
    TimeAxis.cs          // 个人时间轴（资源抽取）
    EventBus.cs          // 事件总线（UI/数据解耦）
  /player
    PlayerState.cs       // 体力、代币、升本经验、水晶耐久
    StaminaSystem.cs     // 体力消耗与回复
    EconomySystem.cs     // 代币、升本、中继经验
    CardSystem.cs        // 抽卡：一页多选、单独付费、开展期保留
  /units
    Unit.cs              // 单位基类
    CooldownSystem.cs    // 冷却计时 + Animator 参数桥
  /logistics
    DeliverySystem.cs    // 相邻传递、运输单位移动
    InterceptionSystem.cs// 嘲讽词条、三方配合、双倍悬赏
    CourierSystem.cs     // 小人：只读访问、合作手操、体力上限
    TopologyGraph.cs     // 固定 2 人线段拓扑（留消除接口）
  /combat
    EnemySpawner.cs      // 出怪、精英密度
    EnemyEvolution.cs    // 全局进化（占位，单机不触发）
    BossSystem.cs        // 小Boss
    CrystalSystem.cs     // 小水晶耐久、出局判定
  /ai
    BotController.cs     // 机器人最简决策
  /data
    MatchRecorder.cs     // 数据表单采集
    LocalDatabase.cs     // JSON 存本地
  /ui
    HUD.cs               // 体力、代币、升本经验、冷却
    CardPanel.cs         // 抽卡面板
    MapView.cs           // 领地 + 小人视图
    SettlementPanel.cs   // 结算数据表单
  /main
    Main.cs              // 入口
```

共 24 个文件。`Config.cs` 集中所有占位数值，方便后续调平衡。

---

## 开发顺序

每步可独立验证，不推翻前一步。

| 步骤 | 内容 | 验证点 |
|---|---|---|
| 1 | 状态机空转 | 状态切换正确，60s 计时准确 |
| 2 | 体力与代币 | 休整/开展回复速率不同，代币按个人时间轴产出 |
| 3 | 抽卡与升本 | 多选扣费、开展期可访问、升本降价提上限 |
| 4 | 单位与敌人 | 部署扣体力、冷却表现、漏怪扣耐久 |
| 5 | 传递与拦截 | 合作触发嘲讽怪、三方配合有收益、不合作小人安全 |
| 6 | 小 Boss | 逃出扣耐久、击杀给赏金 + 升本经验 |
| 7 | 机器人 | 不挂机、不恶意不合作、能基本防守 |
| 8 | 数据表单 | 采集、显示、本地 JSON 存取 |

**第 4 步单位攻击方式**：原型默认**自动攻击**（射程内自动打），不做点击瞄准，因为核心验证点是传递/拦截/三方配合，不是战斗操作。

---

## 工作流

**注释引导 tab 补全 + 人工审查**。

每个文件顶部写规则和依赖，方法前写 TODO 和返回值。tab 补全生成方法体后，人工审查三点：

1. 数值是否从 `Config.cs` 取，没有硬编码。
2. 状态修改是否通过 `EventBus` 通知 UI，没有直接操作 UI。
3. 是否修改了不该修改的状态（比如 UI 层直接改体力）。

**动画层原则**：冷却由 C# 计时器判定，Animator 只接收参数表现。不用动画 Event 回调驱动游戏逻辑。冷却时长和动画时长从同一个数值源取。

---

## 占位数值（`Config.cs`）

```csharp
public static class Config {
    // 时间
    public const float RestPhaseDuration = 60f;
    public const float CombatPhaseDuration = 60f;

    // 体力
    public const float StaminaMax = 100f;
    public const float StaminaRestRestore = 20f;
    public const float StaminaCombatRestore = 2f;
    public const float StaminaDeploy = 10f;
    public const float StaminaMove = 5f;
    public const float StaminaCoopPerSec = 5f;
    public const float CourierStaminaMax = 50f;

    // 经济
    public const int CoinsBasePerRound = 10;
    public const int CoinsVariance = 3;
    public const int CardPriceBase = 5;
    public const int CardPageSizeBase = 3;
    public const int CardPageSizeMax = 6;

    // 升本
    public const int AscendExpPerLevel = 10;
    public const int AscendCardDiscount = 1;
    public const int RelayExpDiscountMax = 3;

    // 敌人
    public const int WaveBaseCount = 5;
    public const int EliteDensityPerEvolution = 1;

    // 小Boss
    public const int MiniBossCrystalDamage = 5;
    public const int MiniBossBounty = 20;

    // 水晶
    public const int CrystalDurabilityMax = 100;
    public const int SmallEnemyCrystalDamage = 1;

    // 资源抽取
    public const float ResourceTickInterval = 10f;
}
```

数值全部是占位，跑起来后调。**不要在这一步纠结平衡**。

---

## 后续路线（不在当前原型内）

1. 联机：先评估 Unity Netcode for GameObjects + Relay 是否够用。
2. 减法地图：出局者格子消除，3 人三角形、2 人线段、1 人单点。
3. 大 Boss：毒圈压缩、设施移动意外事件、大水晶。
4. 评级与称号：基于数据表单的多维评级，历史记录页。
5. 困难/终极模式：入口轮换、精英强化、软对抗积分榜。

---

## 一句话设计原则

**合作是经济选择，不是道德义务。** 体力是唯一成本，传递是交易，拦截是风险，中继是收益。不合作可以玩，只是慢一点。
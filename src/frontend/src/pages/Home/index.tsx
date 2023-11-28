import styles from "./index.module.css";
import useLocalStorage from "../../hooks/useLocalStorage";
import CircleProgress from "../../components/CircleProgress";

export default function Index() {
    return (
        <div className={styles.page}>
            <div className={styles.systemPanel}>
                <div className={styles.panelHeader}>
                    <div className={styles.label}>
                        <div className={styles.panelLabel}>Панель управления</div>
                        <div className={styles.safetyStatus}>Ваш статус безопастности: </div>
                    </div>
                    <button className={styles.updateSafetyStatus}>Повторить анализ</button>
                </div>
                <div className={styles.analytics}>
                    <div className={styles.leftBlock}>
                        <div className={styles.passwordCounterBlock}>
                            <div className={styles.counterLabel}>
                                Кол-во паролей:
                            </div>
                            <div className={styles.passwordCounterValue}>
                                <span>320</span>
                            </div>
                        </div>
                    </div>
                    <div className={styles.visualizationBlock}>
                        <div className={styles.visualizationItem}>
                            <CircleProgress percent={10} notProgressColor={"white"} progressColor={"red"}/>
                            <span>Украденные</span>
                        </div>
                        <div className={styles.visualizationItem}>
                            <CircleProgress percent={10} notProgressColor={"white"} progressColor={"#fee10f"}/>
                            <span>Ненадежные</span>
                        </div>
                        <div className={styles.visualizationItem}>
                            <CircleProgress percent={10} notProgressColor={"white"} progressColor={"green"}/>
                            <span>Надежные</span>
                        </div>
                    </div>
                </div>
                <div className={styles.passwordExplorer}>
                    
                </div>
            </div>
        </div>
    );
};
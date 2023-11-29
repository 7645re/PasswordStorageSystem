import styles from "./index.module.css";
import useLocalStorage from "../../hooks/useLocalStorage";

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
                    <div className={styles.passwordIndicators}>
                        <div className={styles.passwordIndicator}>
                            <div className={`${styles.indicatorCircle} ${styles.greenIndicator}`}>
                                <span>78</span>
                            </div>
                            <span className={styles.indicatorLabel}>Надежные</span>
                        </div>
                        <div className={styles.passwordIndicator}>
                            <div className={`${styles.indicatorCircle} ${styles.yellowIndicator}`}>
                                <span>78</span>
                            </div>
                            <span className={styles.indicatorLabel}>Устаревшие или слабые</span>
                        </div>
                        <div className={styles.passwordIndicator}>
                            <div className={`${styles.indicatorCircle} ${styles.redIndicator}`}>
                                <span>78</span>
                            </div>
                            <span className={styles.indicatorLabel}>Украденные</span>
                        </div>
                    </div>
                </div>
                <div className={styles.credentialsExplorer}>
                    <div className={styles.addCredential}>
                        
                    </div>
                </div>
            </div>
        </div>
    );
};
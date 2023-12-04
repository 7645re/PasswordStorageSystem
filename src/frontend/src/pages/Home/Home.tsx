import styles from "./Home.module.css";
import SystemPanel from "./SystemPanel/SystemPanel";

export function Home() {
    return (
        <div className={styles.page}>
            <SystemPanel/>
        </div>
    );
};
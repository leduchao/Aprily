import { useEffect } from "react";
import { toast } from "../../utils/toast";
import { chatThreads } from "../../data/chat";
import { HomeHeader } from "../../components/home/home-header/index-v2";
import { ThreadList } from "../../components/thread/thread-list/index-v2";
import { NavigationFooter } from "../../components/home/navigation-footer/index2";

export default function HomePage() {
  useEffect(() => {
    toast.showToast();
  }, []);

  const handleSearchThreads = (keyword: string) => {
    console.log(`Searching for ${keyword}...`);
  };

  return (
    <div className="vh-100 overflow-hidden d-flex flex-column">
      <HomeHeader onSearch={handleSearchThreads} />

      <ThreadList threads={chatThreads} />

      <NavigationFooter />
    </div>
  );
}

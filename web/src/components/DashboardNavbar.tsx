import { chakra } from "@chakra-ui/system";
import React, { PropsWithChildren } from "react";
import { Tabs, TabList, Tab } from "@chakra-ui/tabs";
import { useRouter } from "next/router";

const DashboardItems = [
  { id: "overview", title: "Overview", href: "/me" },
  { id: "audios", title: "Uploads", href: "/me/audios" },
  {
    id: "favoriteAudios",
    title: "Favorite Audios",
    href: "/me/favorites/audios",
  },
] as const;

interface DashboardProps {
  active: typeof DashboardItems[number]["id"];
}

export default function UserDashboardSubHeader(
  props: PropsWithChildren<DashboardProps>
) {
  const router = useRouter();
  const activeIndex = DashboardItems.findIndex((x) => x.id === props.active);
  return (
    <chakra.div
      display="flex"
      alignItems="center"
      justifyContent="space-between"
      borderWidth={0}
      overflowX="auto"
      boxShadow="md"
    >
      <Tabs
        colorScheme="primary"
        index={activeIndex}
        borderBottomColor="transparent"
      >
        <TabList>
          {DashboardItems.map((item) => (
            <Tab
              key={item.id}
              onClick={() => router.push(item.href)}
              py={4}
              m={0}
              _focus={{ boxShadow: "none" }}
            >
              {item.title}
            </Tab>
          ))}
        </TabList>
      </Tabs>
    </chakra.div>
  );
}

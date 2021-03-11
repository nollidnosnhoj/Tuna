import { Button, ButtonProps } from "@chakra-ui/button";
import { Box } from "@chakra-ui/layout";
import NextLink from "next/link";
import React from "react";

interface HeaderMenuLinkProps {
  label: string;
  href: string;
  icon?: React.ReactElement<any, string | React.JSXElementConstructor<any>>;
  hidden?: boolean;
}

export default function HeaderMenuLink(
  props: HeaderMenuLinkProps & ButtonProps
) {
  const { label, href, icon, hidden = false, ...buttonProps } = props;

  if (hidden) return null;

  return (
    <NextLink href={href}>
      <Button
        leftIcon={icon}
        width="100%"
        marginY={1}
        paddingY={6}
        paddingX={6}
        borderRadius={0}
        justifyContent="normal"
        variant="ghost"
        {...buttonProps}
      >
        {label}
      </Button>
    </NextLink>
  );
}

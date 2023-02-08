import { ReactNode } from 'react';

interface Props {
  title: string;
  children?: ReactNode;
}

export function DetailItem(props: Props) {
  const { title, children } = props;
  return (
    <div>
      <div>{title}</div>
      <div>{children}</div>
    </div>
  );
}
